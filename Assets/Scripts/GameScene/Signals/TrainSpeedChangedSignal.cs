namespace GameScene.Signals
{
	public sealed class TrainSpeedChangedSignal
	{
		public int Id { get; }
		public float NewSpeed { get; }

		public TrainSpeedChangedSignal(int id, float newSpeed)
		{
			Id = id;
			NewSpeed = newSpeed;
		}
	}
}