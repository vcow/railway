using EditorScene.Graph;

namespace EditorScene.Signals
{
	public sealed class SetConnectionDistanceSignal
	{
		public NodeConnection NodeConnection { get; }
		public float Distance { get; }

		public SetConnectionDistanceSignal(NodeConnection nodeConnection, float distance)
		{
			NodeConnection = nodeConnection;
			Distance = distance;
		}
	}
}