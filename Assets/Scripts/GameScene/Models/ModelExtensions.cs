using System.Collections.Generic;
using System.Linq;

namespace GameScene.Models
{
	public static class ModelExtensions
	{
		public static bool IsNotReserved(this IMineVertexModel mineModel) => mineModel.ReservedByTrain < 0;

		public static IEnumerable<IEnumerable<IGraphVertex>> GetAllVertices(this IGameModel game)
		{
			yield return game.Bases;
			yield return game.Mines;
			yield return game.Nodes;
		}

		public static IGraphVertex GetVertex(this IGameModel game, int id) => game.GetAllVertices()
			.SelectMany(vertices => vertices).FirstOrDefault(vertex => vertex.Id == id);
	}
}