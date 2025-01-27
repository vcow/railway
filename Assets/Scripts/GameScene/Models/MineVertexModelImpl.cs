using System;
using Models;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameScene.Models
{
	public class MineVertexModelImpl : IMineVertexModel, IDisposable
	{
		private readonly CompositeDisposable _disposables;

		public BoolReactiveProperty IsBusy;

		public int Id { get; }
		public Vector2 Position { get; }
		public string Name { get; }
		
		IReadOnlyReactiveProperty<bool> IGraphVertex.IsBusy => IsBusy;

		public MineVertexModelImpl(INodeModel nodeModel)
		{
			Assert.IsTrue(nodeModel.Type == NodeType.Mine);

			Id = nodeModel.Id;
			Position = new Vector2(nodeModel.XPos, nodeModel.YPos);
			Name = nodeModel.Name;
			IsBusy = new BoolReactiveProperty(false);

			_disposables = new CompositeDisposable(IsBusy);
		}

		void IDisposable.Dispose()
		{
			_disposables.Dispose();
		}
	}
}