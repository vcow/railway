namespace GameScene.Models
{
	public interface IConnectionEdgeModel
	{
		float Length { get; }
		int FromNodeId { get; }
		int ToNodeId { get; }
	}
}