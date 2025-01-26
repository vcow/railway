using EditorScene.Graph;

namespace EditorScene.Signals
{
	public sealed class ChangeMarkerMultiplierSignal
	{
		public Marker Marker { get; }
		public float NewMultiplier { get; }

		public ChangeMarkerMultiplierSignal(Marker marker, float newMultiplier)
		{
			Marker = marker;
			NewMultiplier = newMultiplier;
		}
	}
}