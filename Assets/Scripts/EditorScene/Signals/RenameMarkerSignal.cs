using EditorScene.Graph;

namespace EditorScene.Signals
{
	public class RenameMarkerSignal
	{
		public Marker Marker { get; }
		public string NewName { get; }

		public RenameMarkerSignal(Marker marker, string newName)
		{
			Marker = marker;
			NewName = newName;
		}
	}
}