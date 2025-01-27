namespace GameScene.Models
{
	public interface IBaseVertexModel : IGraphVertex
	{
		string Name { get; }
		float Multiplier { get; }
	}
}