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

		public float LastMiningTime;
		public int ReservedByTrain = -1;
		public bool IsBusy;
		public readonly BoolReactiveProperty IsMining;
		public readonly FloatReactiveProperty Progress;

		public int Id { get; }
		public Vector2 Position { get; }
		public string Name { get; }
		public float Multiplier { get; private set; }
		float IMineVertexModel.LastMiningTime => LastMiningTime;
		int IMineVertexModel.ReservedByTrain => ReservedByTrain;
		bool IGraphVertex.IsBusy => IsBusy;
		IReadOnlyReactiveProperty<bool> IMineVertexModel.IsMining => IsMining;
		IReadOnlyReactiveProperty<float> IMineVertexModel.Progress => Progress;

		public MineVertexModelImpl(INodeModel nodeModel, SignalBus signalBus)
		{
			Assert.IsTrue(nodeModel.Type == NodeType.Mine);

			Id = nodeModel.Id;
			Position = new Vector2(nodeModel.XPos, nodeModel.YPos);
			Name = nodeModel.Name;
			Multiplier = nodeModel.Multiplier;
			IsMining = new BoolReactiveProperty(false);
			Progress = new FloatReactiveProperty(0f);

			_disposables = new CompositeDisposable(IsMining, Progress);

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