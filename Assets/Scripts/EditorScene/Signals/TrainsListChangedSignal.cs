using System.Collections.Generic;
using Models;

namespace EditorScene.Signals
{
	public sealed class TrainsListChangedSignal
	{
		public IReadOnlyList<TrainModelImpl> Trains { get; }

		public TrainsListChangedSignal(IReadOnlyList<TrainModelImpl> trains)
		{
			Trains = trains;
		}
	}
}