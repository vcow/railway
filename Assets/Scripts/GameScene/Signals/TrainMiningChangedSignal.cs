namespace GameScene.Signals
{
	public class TrainMiningChangedSignal
	{
		public int Id { get; }
		public float NewMining { get; }

		public TrainMiningChangedSignal(int id, float newMining)
		{
			Id = id;
			NewMining = newMining;
		}
	}
}