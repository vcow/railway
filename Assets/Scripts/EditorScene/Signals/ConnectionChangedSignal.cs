using EditorScene.Graph;

namespace EditorScene.Signals
{
	public sealed class ConnectionChangedSignal
	{
		public NodeConnection NodeConnection { get; }

		public ConnectionChangedSignal(NodeConnection nodeConnection)
		{
			NodeConnection = nodeConnection;
		}
	}
}