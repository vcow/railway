using EditorScene.Graph;

namespace EditorScene.Signals
{
	public sealed class SelectConnectionSignal
	{
		public NodeConnection NodeConnection { get; }

		public SelectConnectionSignal(NodeConnection nodeConnection)
		{
			NodeConnection = nodeConnection;
		}
	}
}