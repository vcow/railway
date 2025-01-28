using System.Collections.Generic;
using System.Linq;
using GameScene.Models;

namespace GameScene.Logic
{
	public sealed class TrainPath
	{
		private readonly IGameModel _gameModel;

		public IReadOnlyList<IGraphVertex> Path { get; }

		public TrainPath(IReadOnlyList<IGraphVertex> path, IGameModel gameModel)
		{
			_gameModel = gameModel;
			Path = path;
		}

		public float GetPathLength(int from, int to)
		{
			var len = 0f;
			int a = -1, b;
			foreach (var vertex in Path)
			{
				if (a < 0)
				{
					if (vertex.Id == from)
					{
						a = vertex.Id;
					}

					continue;
				}

				b = vertex.Id;
				var connection = _gameModel.Connections.First(c => c.FromNodeId == a && c.ToNodeId == b || c.FromNodeId == b && c.ToNodeId == a);
				len += connection.Length;
				a = b;

				if (b == to)
				{
					break;
				}
			}

			return len;
		}

		public float GetPathDistance(int from, int to)
		{
			var len = 0f;
			IGraphVertex a = null, b;
			foreach (var vertex in Path)
			{
				if (a == null)
				{
					if (vertex.Id == from)
					{
						a = vertex;
					}

					continue;
				}

				b = vertex;
				var delta = b.Position - a.Position;
				len += delta.magnitude;
				a = b;

				if (b.Id == to)
				{
					break;
				}
			}

			return len;
		}
	}
}