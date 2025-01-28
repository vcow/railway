using System;
using GameScene.Signals;
using Models;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Models
{
	public class BaseVertexModelImpl : IBaseVertexModel, IDisposable
	{
		private readonly CompositeDisposable _disposables;
		private readonly SignalBus _signalBus;

		public bool IsBusy;
		public readonly FloatReactiveProperty Resources;

		public int Id { get; }
		public Vector2 Position { get; }
		public string Name { get; }
		public float Multiplier { get; private set; }
		bool IGraphVertex.IsBusy => IsBusy;
		IReadOnlyReactiveProperty<float> IBaseVertexModel.Resources => Resources;

		public BaseVertexModelImpl(INodeModel nodeModel, SignalBus signalBus)
		{
			Assert.IsTrue(nodeModel.Type == NodeType.Base);

			Id = nodeModel.Id;
			Position = new Vector2(nodeModel.XPos, nodeModel.YPos);
			Name = nodeModel.Name;
			Multiplier = nodeModel.Multiplier;
			Resources = new FloatReactiveProperty(0f);

			_disposables = new CompositeDisposable(Resources);

			_signalBus = signalBus;
			_signalBus.Subscribe<VertexMultiplierChangedSignal>(OnVertexMultiplierChanged);
		}

		private void OnVertexMultiplierChanged(VertexMultiplierChangedSignal signal)
		{
			if (signal.Id == Id)
			{
				Multiplier = signal.NewMultiplier;
			}
		}

		void IDisposable.Dispose()
		{
			_disposables.Dispose();
			_signalBus.Unsubscribe<VertexMultiplierChangedSignal>(OnVertexMultiplierChanged);
		}
	}
}