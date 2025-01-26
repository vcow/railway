using EditorScene.Graph;

namespace EditorScene.Signals
{
	public sealed class RemoveConnectionSignal
	{
		public NodeConnection NodeConnection { get; }

		public RemoveConnectionSignal(NodeConnection nodeConnection)
		{
			NodeConnection = nodeConnection;
		}
	}
}