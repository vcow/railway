using UniRx;

namespace GameScene.Models
{
	public interface IMineVertexModel : IGraphVertex
	{
		string Name { get; }
		float Multiplier { get; }
		float LastMiningTime { get; }
		int ReservedByTrain { get; }
		IReadOnlyReactiveProperty<bool> IsMining { get; }
		IReadOnlyReactiveProperty<float> Progress { get; }
	}
}