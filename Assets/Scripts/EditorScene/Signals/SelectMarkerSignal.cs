using EditorScene.Graph;

namespace EditorScene.Signals
{
	public class SelectMarkerSignal
	{
		public Marker Marker { get; }

		public SelectMarkerSignal(Marker marker)
		{
			Marker = marker;
		}
	}
}