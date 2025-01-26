using EditorScene.Graph;

namespace EditorScene.Signals
{
	public sealed class MarkForConnectionSignal
	{
		public Marker Marker { get; }

		public MarkForConnectionSignal(Marker marker)
		{
			Marker = marker;
		}
	}
}