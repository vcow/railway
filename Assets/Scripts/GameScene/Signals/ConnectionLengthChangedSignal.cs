namespace GameScene.Signals
{
	public sealed class ConnectionLengthChangedSignal
	{
		public int FromNodeId { get; }
		public int ToNodeId { get; }
		public float NewLength { get; }

		public ConnectionLengthChangedSignal(int fromNodeId, int toNodeId, float newLength)
		{
			FromNodeId = fromNodeId;
			ToNodeId = toNodeId;
			NewLength = newLength;
		}
	}
}