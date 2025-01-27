using System.Collections.Generic;

namespace GameScene.Models
{
	public interface IGameModel
	{
		IReadOnlyList<IBaseVertexModel> Bases { get; }
		IReadOnlyList<IMineVertexModel> Mines { get; }
		IReadOnlyList<INodeVertexModel> Nodes { get; }
	}
}