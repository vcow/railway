using System;
using GameScene.Signals;
using Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace GameScene.Models
{
	public class TrainObjectModelImpl : ITrainObjectModel, IDisposable
	{
		private readonly CompositeDisposable _disposables;
		private readonly SignalBus _signalBus;

		public ReactiveProperty<Vector2> Position;

		public int Id { get; }
		public string Name { get; }
		public float Speed { get; private set; }
		public float Mining { get; private set; }
		IReadOnlyReactiveProperty<Vector2> ITrainObjectModel.Position => Position;

		public TrainObjectModelImpl(int id, ITrainModel trainModel, Vector2 initialPosition, SignalBus signalBus)
		{
			Id = id;
			Name = trainModel.Name;
			Speed = trainModel.Speed;
			Mining = trainModel.Mining;
			Position = new ReactiveProperty<Vector2>(initialPosition);

			_disposables = new CompositeDisposable(Position);

			_signalBus = signalBus;
			_signalBus.Subscribe<TrainSpeedChangedSignal>(OnTrainSpeedChanged);
			_signalBus.Subscribe<TrainMiningChangedSignal>(OnTrainMiningChanged);
		}

		private void OnTrainSpeedChanged(TrainSpeedChangedSignal signal)
		{
			if (signal.Id == Id)
			{
				Speed = signal.NewSpeed;
			}
		}

		private void OnTrainMiningChanged(TrainMiningChangedSignal signal)
		{
			if (signal.Id == Id)
			{
				Mining = signal.NewMining;
			}
		}

		void IDisposable.Dispose()
		{
			_disposables.Dispose();
			_signalBus.Unsubscribe<TrainSpeedChangedSignal>(OnTrainSpeedChanged);
			_signalBus.Unsubscribe<TrainMiningChangedSignal>(OnTrainMiningChanged);
		}
	}
}