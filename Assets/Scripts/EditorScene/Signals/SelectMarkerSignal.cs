using EditorScene.Graph;

namespace EditorScene.Signals
{
	public sealed class SelectMarkerSignal
	{
		public Marker Marker { get; }

		public SelectMarkerSignal(Marker marker)
		{
			Marker = marker;
		}
	}
}