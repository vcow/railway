using EditorScene.Graph;

namespace EditorScene.Signals
{
	public sealed class RemoveMarkerSignal
	{
		public Marker Marker { get; }

		public RemoveMarkerSignal(Marker marker)
		{
			Marker = marker;
		}
	}
}