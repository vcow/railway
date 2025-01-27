namespace GameScene.Signals
{
	public class VertexMultiplierChangedSignal
	{
		public int Id { get; }
		public float NewMultiplier { get; }

		public VertexMultiplierChangedSignal(int id, float newMultiplier)
		{
			Id = id;
			NewMultiplier = newMultiplier;
		}
	}
}