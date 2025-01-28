using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameScene.Controllers;
using GameScene.Models;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Logic
{
	public sealed class GameLogic : ITickable, IDisposable
	{
		private const float TrainSpeedMultiplier = 0.3f;

		private readonly GameModelController _gameModelController;
		private readonly Queue<ITrainObjectModel> _freeTrains;
		private readonly Dictionary<int, VertexScheduler> _schedulers = new();

		public GameLogic(GameModelController gameModelController)
		{
			_gameModelController = gameModelController;
			_freeTrains = new Queue<ITrainObjectModel>(_gameModelController.GameModel.Trains);
		}

		void IDisposable.Dispose()
		{
		}

		void ITickable.Tick()
		{
			if (_freeTrains.TryDequeue(out var freeTrain))
			{
				ExecuteTrain(freeTrain);
			}
		}

		private async void ExecuteTrain(ITrainObjectModel train)
		{
			Assert.IsTrue(train.State == TrainState.Free);

			var freeMines = _gameModelController.GameModel.Mines
				.Where(mine => mine.IsNotReserved()).OrderBy(mine => mine.LastMiningTime);
			var currentTrainVertex = _gameModelController.GameModel.GetVertex(train.DestinationId);
			TrainPath path = null;
			foreach (var mine in freeMines)
			{
				Assert.IsTrue(train.DestinationId != mine.Id);
				path = PathFinder.FindPath(currentTrainVertex, mine, _gameModelController.GameModel);
				if (path != null)
				{
					break;
				}
			}

			if (path == null)
			{
				// No available job.
				await UniTask.Yield(PlayerLoopTiming.Update);
				_freeTrains.Enqueue(train);
				return;
			}

			// Go to mine
			var targetMine = (IMineVertexModel)path.Path.Last();
			Debug.Log($"Train {train.Name} goes to the {targetMine.Id}.");
			_gameModelController.SetTrainDestination(train, targetMine);
			await MoveToDestination(train, path);
			Assert.IsTrue(targetMine.ReservedByTrain == train.Id && targetMine.IsBusy);

			// Mining
			await Mine(targetMine, train);

			// Go to base
			do
			{
				path = FindPathToNearestBase(targetMine);
				if (path == null)
				{
					Debug.Log($"Can't find path from mine {targetMine} to the any base.");
					await UniTask.Yield(PlayerLoopTiming.Update);
				}
			} while (path == null);

			var targetBase = (IBaseVertexModel)path.Path.Last();
			Debug.Log($"Path from mine {targetMine.Id} to the base {targetBase.Id} was found");
			_gameModelController.SetTrainDestination(train, targetBase);
			await MoveToDestination(train, path);
			targetBase = (IBaseVertexModel)path.Path.Last();
			Assert.IsTrue(targetBase.IsBusy);

			// Unload
			await Unload(targetBase, train);

			// Finish
			Debug.Log($"Iteration for the train {train.Name} is finished.");
			_freeTrains.Enqueue(train);
		}

		private async UniTask MoveToDestination(ITrainObjectModel train, TrainPath path)
		{
			using var pathEnumerator = path.Path.GetEnumerator();
			if (!pathEnumerator.MoveNext() || pathEnumerator.Current == null)
			{
				throw new Exception("Empty path received.");
			}

			Assert.IsTrue(train.Speed > 0f);
			var currentVertex = pathEnumerator.Current;
			var wholePathLength = path.GetPathLength(currentVertex.Id, path.Path.Last().Id);
			var impliedTravelTime = wholePathLength / train.Speed;
			var wholePathDistance = path.GetPathDistance(currentVertex.Id, path.Path.Last().Id);
			var actualTrainSpeed = wholePathDistance / impliedTravelTime * TrainSpeedMultiplier;

			while (pathEnumerator.MoveNext())
			{
				Assert.IsTrue(currentVertex.IsBusy);

				Debug.Log($"Move train {train.Name} from {currentVertex.Id} to {pathEnumerator.Current.Id}.");

				var checkTimestamp = Time.time;
				while (pathEnumerator.Current.IsBusy)
				{
					// Destination vertex is locked
					if (Time.time - checkTimestamp < VertexScheduler.TimeGap)
					{
						// Wait a bit
						await UniTask.Yield(PlayerLoopTiming.Update);
						continue;
					}

					Debug.LogWarning($"Destination vertex {pathEnumerator.Current.Id} is busy for {VertexScheduler.TimeGap} seconds.");
					var targetVertex = path.Path.Last();
					// If the train was going to the base, try to find any other free base, or try to find another way
					var newPath = targetVertex is IBaseVertexModel
						? FindPathToNearestBase(currentVertex)
						: PathFinder.FindPath(currentVertex, targetVertex, _gameModelController.GameModel);
					if (newPath != null)
					{
						// Go another way
						targetVertex = newPath.Path.Last();
						Debug.Log($"New way has been chosen. Destination vertex is {targetVertex.Id}.");
						_gameModelController.SetTrainDestination(train, targetVertex);
						await MoveToDestination(train, newPath);
						return;
					}

					// Wait
					Debug.LogWarning("Can't find path. Wait.");
					await UniTask.Yield(PlayerLoopTiming.Update);
				}

				var scheduler = GetScheduler(pathEnumerator.Current.Id);
				var distance = path.GetPathDistance(currentVertex.Id, pathEnumerator.Current.Id);
				var travelTime = distance / actualTrainSpeed;
				var estimatedArrivalTime = travelTime + Time.time;
				var actualArrivalTime = scheduler.ScheduleArrival(estimatedArrivalTime);
				if (!actualArrivalTime.Equals(estimatedArrivalTime))
				{
					Assert.IsTrue(actualArrivalTime > estimatedArrivalTime);

					// Destination vertex is reserved
					var delayTime = actualArrivalTime - estimatedArrivalTime;
					Debug.Log($"Destination vertex {pathEnumerator.Current.Id} is reserved. Available time: now + {delayTime}. Wait...");
					await UniTask.WaitForSeconds(delayTime);
					Debug.Log("... Continue.");
				}

				_gameModelController.SetVertexBusy(currentVertex, false);

				float timesLeft;
				Debug.Log($"Start train {train.Name} motion from {currentVertex.Id} to {pathEnumerator.Current.Id}.");
				while ((timesLeft = actualArrivalTime - Time.time) > 0)
				{
					// Move train
					_gameModelController.SetTrainPosition(train, Vector2.Lerp(pathEnumerator.Current.Position,
						currentVertex.Position, Mathf.Clamp01(timesLeft / travelTime)));
					await UniTask.Yield(PlayerLoopTiming.Update);
				}

				currentVertex = pathEnumerator.Current;
				_gameModelController.SetVertexBusy(currentVertex, true);
				Debug.Log($"Vertex {currentVertex.Id} has been reached by train {train.Name}.");
			}
		}

		private async UniTask Mine(IMineVertexModel mine, ITrainObjectModel train)
		{
			_gameModelController.SetIsMining(mine, train, true);

			var mineTime = train.Mining * mine.Multiplier;
			Debug.Log($"Start mining by train {train.Name}. Mine time is {mineTime} sec.");
			var finishTime = Time.time + mineTime;
			float deltaTime;
			while ((deltaTime = finishTime - Time.time) >= 0)
			{
				_gameModelController.SetProgress(mine, 1f - Mathf.Clamp01(deltaTime / mineTime));
				await UniTask.Yield(PlayerLoopTiming.Update);
			}

			_gameModelController.SetIsMining(mine, train, false);
			Debug.Log($"Mining by train {train.Name} is finished.");
		}

		private UniTask Unload(IBaseVertexModel baseModel, ITrainObjectModel train)
		{
			const float resourcesFromTrain = 1f;
			var resourcesReceived = resourcesFromTrain * baseModel.Multiplier;
			Debug.Log($"Unload resources {resourcesReceived} from the train {train.Name} to the base {baseModel.Id}");
			_gameModelController.SetResourcesToBase(baseModel, train, resourcesReceived);
			return UniTask.CompletedTask;
		}

		private TrainPath FindPathToNearestBase(IGraphVertex from)
		{
			Assert.IsFalse(from is IBaseVertexModel);

			const float keyStep = 0.001f;
			var paths = new SortedList<float, TrainPath>();
			var freeBases = _gameModelController.GameModel.Bases
				.Where(baseModel => !baseModel.IsBusy);
			foreach (var to in freeBases)
			{
				var path = PathFinder.FindPath(from, to, _gameModelController.GameModel);
				if (path == null)
				{
					continue;
				}

				var key = path.GetPathLength(from.Id, to.Id);
				while (paths.Keys.Contains(key))
				{
					key += keyStep;
				}

				paths.Add(key, path);
			}

			return paths.Any() ? paths.First().Value : null;
		}

		private VertexScheduler GetScheduler(int vertexId)
		{
			if (!_schedulers.TryGetValue(vertexId, out var scheduler))
			{
				scheduler = new VertexScheduler();
				_schedulers.Add(vertexId, scheduler);
			}

			return scheduler;
		}
	}
}