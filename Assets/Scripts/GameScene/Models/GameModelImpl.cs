using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using UniRx;
using Zenject;
using Random = UnityEngine.Random;

namespace GameScene.Models
{
	public sealed class GameModelImpl : IGameModel, IDisposable
	{
		private readonly CompositeDisposable _disposables = new();

		public IReadOnlyList<IBaseVertexModel> Bases { get; }
		public IReadOnlyList<IMineVertexModel> Mines { get; }
		public IReadOnlyList<INodeVertexModel> Nodes { get; }
		public IReadOnlyList<IConnectionEdgeModel> Connections { get; }
		public IReadOnlyList<ITrainObjectModel> Trains { get; }

		public GameModelImpl(ILevelModel levelModel, SignalBus signalBus)
		{
			var bases = new List<IBaseVertexModel>();
			var mines = new List<IMineVertexModel>();
			var nodes = new List<INodeVertexModel>();
			var connections = new List<IConnectionEdgeModel>();
			var trains = new List<ITrainObjectModel>();

			foreach (var nodeModel in levelModel.Nodes)
			{
				switch (nodeModel.Type)
				{
					case NodeType.Node:
						nodes.Add(new NodeVertexModelImpl(nodeModel).AddTo(_disposables));
						break;
					case NodeType.Mine:
						mines.Add(new MineVertexModelImpl(nodeModel, signalBus).AddTo(_disposables));
						break;
					case NodeType.Base:
						bases.Add(new BaseVertexModelImpl(nodeModel, signalBus).AddTo(_disposables));
						break;
					default:
						throw new NotSupportedException();
				}
			}

			foreach (var connectionModel in levelModel.Connections)
			{
				connections.Add(new ConnectionEdgeModelImpl(connectionModel, signalBus).AddTo(_disposables));
			}

			var trainId = 0;
			var freeBases = new List<IBaseVertexModel>(bases);
			foreach (var trainModel in levelModel.Trains)
			{
				if (!freeBases.Any())
				{
					break;
				}

				var randomBase = freeBases[Random.Range(0, freeBases.Count)];
				freeBases.Remove(randomBase);

				trains.Add(new TrainObjectModelImpl(trainId++, trainModel, randomBase.Position, signalBus).AddTo(_disposables));
			}

			Bases = bases;
			Mines = mines;
			Nodes = nodes;
			Connections = connections;
			Trains = trains;
		}

		void IDisposable.Dispose()
		{
			_disposables.Dispose();
		}
	}
}