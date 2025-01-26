using EditorScene.Graph;

namespace EditorScene.Signals
{
	public sealed class DragMarkerSignal
	{
		public Marker Marker { get; }

		public DragMarkerSignal(Marker marker)
		{
			Marker = marker;
		}
	}
}