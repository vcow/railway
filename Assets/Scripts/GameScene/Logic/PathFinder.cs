using System.Collections.Generic;
using System.Linq;
using GameScene.Models;
using UnityEngine;

namespace GameScene.Logic
{
	public static class PathFinder
	{
		private class PathNode
		{
			public IGraphVertex Position;
			public float PathLengthFromStart;
			public PathNode CameFrom;
			public float HeuristicEstimatePathLen;
			public float EstimateFullPathLen => PathLengthFromStart + HeuristicEstimatePathLen;
		}

		public static TrainPath FindPath(IGraphVertex from, IGraphVertex to, IGameModel gameModel)
		{
			var closedSet = new HashSet<PathNode>();
			var openSet = new HashSet<PathNode>();
			var startNode = new PathNode
			{
				Position = from,
				CameFrom = null,
				PathLengthFromStart = 0f,
				HeuristicEstimatePathLen = GetHeuristicPathLength(from.Position, to.Position)
			};
			openSet.Add(startNode);

			var neighbours = new List<PathNode>();
			while (openSet.Any())
			{
				var currentNode = openSet.OrderBy(node => node.EstimateFullPathLen).First();
				if (currentNode.Position == to)
				{
					return GetPathForNode(currentNode, gameModel);
				}

				openSet.Remove(currentNode);
				closedSet.Add(currentNode);

				GetNeighbours(currentNode, to, gameModel, neighbours);
				foreach (var neighbourNode in neighbours)
				{
					if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
					{
						continue;
					}

					var openNode = openSet.FirstOrDefault(node => node.Position == neighbourNode.Position);
					if (openNode == null)
					{
						openSet.Add(neighbourNode);
					}
					else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
					{
						openNode.CameFrom = currentNode;
						openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
					}
				}
			}

			return null;
		}

		private static float GetHeuristicPathLength(Vector2 from, Vector2 to) =>
			Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);

		private static TrainPath GetPathForNode(PathNode node, IGameModel gameModel)
		{
			var result = new List<IGraphVertex>(16);
			do
			{
				result.Add(node.Position);
				node = node.CameFrom;
			} while (node != null);

			result.Reverse();
			return new TrainPath(result, gameModel);
		}

		private static void GetNeighbours(PathNode node, IGraphVertex to,
			IGameModel gameModel, List<PathNode> neighbours)
		{
			neighbours.Clear();
			var nodeVertexId = node.Position.Id;
			foreach (var connection in gameModel.Connections)
			{
				int neighbourId;
				if (connection.FromNodeId == nodeVertexId)
				{
					neighbourId = connection.ToNodeId;
				}
				else if (connection.ToNodeId == nodeVertexId)
				{
					neighbourId = connection.FromNodeId;
				}
				else
				{
					continue;
				}

				var neighbourVertex = gameModel.GetVertex(neighbourId);
				if (neighbourVertex.IsBusy)
				{
					continue;
				}

				neighbours.Add(new PathNode
				{
					Position = neighbourVertex,
					CameFrom = node,
					PathLengthFromStart = node.PathLengthFromStart + connection.Length,
					HeuristicEstimatePathLen = GetHeuristicPathLength(neighbourVertex.Position, to.Position)
				});
			}
		}
	}
}