using EditorScene.Graph;

namespace EditorScene.Signals
{
	public class RemoveMarkerSignal
	{
		public Marker Marker { get; }

		public RemoveMarkerSignal(Marker marker)
		{
			Marker = marker;
		}
	}
}