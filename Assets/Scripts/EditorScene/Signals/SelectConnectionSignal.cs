using EditorScene.Graph;

namespace EditorScene.Signals
{
	public class SelectConnectionSignal
	{
		public NodeConnection NodeConnection { get; }

		public SelectConnectionSignal(NodeConnection nodeConnection)
		{
			NodeConnection = nodeConnection;
		}
	}
}