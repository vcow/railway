using System;
using GameScene.Signals;
using Models;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Models
{
	public class MineVertexModelImpl : IMineVertexModel, IDisposable
	{
		private readonly CompositeDisposable _disposables;
		private readonly SignalBus _signalBus;

		public BoolReactiveProperty IsBusy;

		public int Id { get; }
		public Vector2 Position { get; }
		public string Name { get; }
		public float Multiplier { get; private set; }
		IReadOnlyReactiveProperty<bool> IGraphVertex.IsBusy => IsBusy;

		public MineVertexModelImpl(INodeModel nodeModel, SignalBus signalBus)
		{
			Assert.IsTrue(nodeModel.Type == NodeType.Mine);

			Id = nodeModel.Id;
			Position = new Vector2(nodeModel.XPos, nodeModel.YPos);
			Name = nodeModel.Name;
			Multiplier = nodeModel.Multiplier;
			IsBusy = new BoolReactiveProperty(false);

			_disposables = new CompositeDisposable(IsBusy);

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