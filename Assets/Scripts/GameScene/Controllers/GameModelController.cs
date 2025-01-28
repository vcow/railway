using System;
using System.Linq;
using GameScene.Logic;
using GameScene.Models;
using Models;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Controllers
{
	public class GameModelController : IDisposable
	{
		private readonly CompositeDisposable _disposables;

		private readonly GameModelImpl _gameModel;

		public IGameModel GameModel => _gameModel;

		public GameModelController(ILevelModel levelModel, SignalBus signalBus)
		{
			_gameModel = new GameModelImpl(levelModel, signalBus);
			_disposables = new CompositeDisposable(_gameModel);
		}

		void IDisposable.Dispose()
		{
			_disposables.Dispose();
		}

		public void SetTrainDestination(int trainId, IGraphVertex destination)
		{
			SetTrainDestination(_gameModel.Trains.First(model => model.Id == trainId), destination);
		}

		public void SetTrainDestination(ITrainObjectModel train, IGraphVertex destination)
		{
			if (_gameModel.GetVertex(train.DestinationId) is MineVertexModelImpl prevDestination)
			{
				Assert.IsTrue(prevDestination.ReservedByTrain == train.Id);
				prevDestination.ReservedByTrain = -1;
			}

			var t = (TrainObjectModelImpl)train;
			t.DestinationId = destination.Id;
			t.State = destination switch
			{
				IBaseVertexModel => TrainState.MoveToBase,
				IMineVertexModel => TrainState.MoveToMine,
				_ => throw new ArgumentException("Wong destination for the train.")
			};

			if (destination is MineVertexModelImpl mine)
			{
				Assert.IsTrue(mine.IsNotReserved());
				mine.ReservedByTrain = train.Id;
			}
		}

		public void SetTrainPosition(int trainId, Vector2 newPosition)
		{
			SetTrainPosition(_gameModel.Trains.First(model => model.Id == trainId), newPosition);
		}

		public void SetTrainPosition(ITrainObjectModel train, Vector2 newPosition)
		{
			((TrainObjectModelImpl)train).Position.Value = newPosition;
		}

		public void SetVertexBusy(int vertexId, bool isBusy)
		{
			SetVertexBusy(_gameModel.GetVertex(vertexId), isBusy);
		}

		public void SetVertexBusy(IGraphVertex vertex, bool isBusy)
		{
			switch (vertex)
			{
				case BaseVertexModelImpl baseModel:
					baseModel.IsBusy = isBusy;
					break;
				case MineVertexModelImpl mineModel:
					mineModel.IsBusy = isBusy;
					break;
				case NodeVertexModelImpl nodeModel:
					nodeModel.IsBusy = isBusy;
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public void SetIsMining(int mineId, int trainId, bool isMining)
		{
			SetIsMining(_gameModel.Mines.First(model => model.Id == mineId),
				_gameModel.Trains.First(model => model.Id == trainId), isMining);
		}

		public void SetIsMining(IMineVertexModel mine, ITrainObjectModel train, bool isMining)
		{
			var m = (MineVertexModelImpl)mine;
			if (m.IsMining.Value && !isMining)
			{
				m.IsMining.Value = false;
				m.LastMiningTime = Time.time;
			}
			else
			{
				m.IsMining.Value = isMining;
			}

			((TrainObjectModelImpl)train).State = isMining ? TrainState.Loading : TrainState.MoveToBase;
		}

		public void SetProgress(int mineId, float progress)
		{
			SetProgress(_gameModel.GetVertex(mineId), progress);
		}

		public void SetProgress(IGraphVertex vertex, float progress)
		{
			switch (vertex)
			{
				case MineVertexModelImpl mineModel:
					mineModel.Progress.Value = progress;
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public void SetResourcesToBase(int baseId, int trainId, float resourcesReceived)
		{
			SetResourcesToBase(_gameModel.Bases.First(model => model.Id == baseId),
				_gameModel.Trains.First(model => model.Id == trainId), resourcesReceived);
		}

		public void SetResourcesToBase(IBaseVertexModel baseModel, ITrainObjectModel train, float resourcesReceived)
		{
			Assert.IsTrue(resourcesReceived >= 0f);
			((BaseVertexModelImpl)baseModel).Resources.Value += resourcesReceived;
			((TrainObjectModelImpl)train).State = TrainState.Free;
		}
	}
}