using EditorScene.Graph;

namespace EditorScene.Signals
{
	public class MarkForConnectionSignal
	{
		public Marker Marker { get; }

		public MarkForConnectionSignal(Marker marker)
		{
			Marker = marker;
		}
	}
}