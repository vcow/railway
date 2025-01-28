using UnityEngine;

namespace GameScene.Models
{
	public interface IGraphVertex
	{
		int Id { get; }
		Vector2 Position { get; }
		bool IsBusy { get; }
	}
}