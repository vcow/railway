using EditorScene.Graph;

namespace EditorScene.Signals
{
	public sealed class SetConnectionLengthSignal
	{
		public NodeConnection NodeConnection { get; }
		public float Length { get; }

		public SetConnectionLengthSignal(NodeConnection nodeConnection, float length)
		{
			NodeConnection = nodeConnection;
			Length = length;
		}
	}
}