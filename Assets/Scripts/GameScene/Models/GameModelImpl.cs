using System;
using System.Collections.Generic;
using Models;
using UniRx;
using Zenject;

namespace GameScene.Models
{
	public sealed class GameModelImpl : IGameModel, IDisposable
	{
		private readonly CompositeDisposable _disposables = new();

		public IReadOnlyList<IBaseVertexModel> Bases { get; }
		public IReadOnlyList<IMineVertexModel> Mines { get; }
		public IReadOnlyList<INodeVertexModel> Nodes { get; }
		public IReadOnlyList<IConnectionEdgeModel> Connections { get; }

		public GameModelImpl(ILevelModel levelModel, SignalBus signalBus)
		{
			var bases = new List<IBaseVertexModel>();
			var mines = new List<IMineVertexModel>();
			var nodes = new List<INodeVertexModel>();
			var connections = new List<IConnectionEdgeModel>();

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

			Bases = bases;
			Mines = mines;
			Nodes = nodes;
			Connections = connections;
		}

		void IDisposable.Dispose()
		{
			_disposables.Dispose();
		}
	}
}