using System;
using GameScene.Signals;
using Models;
using Zenject;

namespace GameScene.Models
{
	public class ConnectionEdgeModelImpl : IConnectionEdgeModel, IDisposable
	{
		private readonly SignalBus _signalBus;

		public float Length { get; private set; }
		public int FromNodeId { get; }
		public int ToNodeId { get; }

		public ConnectionEdgeModelImpl(IConnectionModel connectionModel, SignalBus signalBus)
		{
			_signalBus = signalBus;
			Length = connectionModel.Length;
			FromNodeId = connectionModel.FromNodeId;
			ToNodeId = connectionModel.ToNodeId;
			_signalBus.Subscribe<ConnectionLengthChangedSignal>(OnConnectionLengthChanged);
		}

		private void OnConnectionLengthChanged(ConnectionLengthChangedSignal signal)
		{
			if (signal.FromNodeId == FromNodeId && signal.ToNodeId == ToNodeId)
			{
				Length = signal.NewLength;
			}
		}

		void IDisposable.Dispose()
		{
			_signalBus.Unsubscribe<ConnectionLengthChangedSignal>(OnConnectionLengthChanged);
		}
	}
}