namespace GameScene.Models
{
	public interface IMineVertexModel : IGraphVertex
	{
		string Name { get; }
		float Multiplier { get; }
	}
}