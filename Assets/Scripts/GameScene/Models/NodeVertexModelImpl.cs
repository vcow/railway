using System;
using Models;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameScene.Models
{
	public sealed class NodeVertexModelImpl : INodeVertexModel, IDisposable
	{
		private readonly CompositeDisposable _disposables;

		public BoolReactiveProperty IsBusy;

		public int Id { get; }
		public Vector2 Position { get; }

		IReadOnlyReactiveProperty<bool> IGraphVertex.IsBusy => IsBusy;

		public NodeVertexModelImpl(INodeModel nodeModel)
		{
			Assert.IsTrue(nodeModel.Type == NodeType.Node);

			Id = nodeModel.Id;
			Position = new Vector2(nodeModel.XPos, nodeModel.YPos);
			IsBusy = new BoolReactiveProperty(false);

			_disposables = new CompositeDisposable(IsBusy);
		}

		void IDisposable.Dispose()
		{
			_disposables.Dispose();
		}
	}
}