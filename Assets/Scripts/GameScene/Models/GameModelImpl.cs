using System;
using System.Collections.Generic;
using Models;
using UniRx;

namespace GameScene.Models
{
	public sealed class GameModelImpl : IGameModel, IDisposable
	{
		private readonly CompositeDisposable _disposables = new();

		public IReadOnlyList<IBaseVertexModel> Bases { get; }
		public IReadOnlyList<IMineVertexModel> Mines { get; }
		public IReadOnlyList<INodeVertexModel> Nodes { get; }

		public GameModelImpl(ILevelModel levelModel)
		{
			var bases = new List<IBaseVertexModel>();
			var mines = new List<IMineVertexModel>();
			var nodes = new List<INodeVertexModel>();

			foreach (var nodeModel in levelModel.Nodes)
			{
				switch (nodeModel.Type)
				{
					case NodeType.Node:
						nodes.Add(new NodeVertexModelImpl(nodeModel).AddTo(_disposables));
						break;
					case NodeType.Mine:
						mines.Add(new MineVertexModelImpl(nodeModel).AddTo(_disposables));
						break;
					case NodeType.Base:
						bases.Add(new BaseVertexModelImpl(nodeModel).AddTo(_disposables));
						break;
					default:
						throw new NotSupportedException();
				}
			}

			Bases = bases;
			Mines = mines;
			Nodes = nodes;
		}

		void IDisposable.Dispose()
		{
			_disposables.Dispose();
		}
	}
}