using GameScene.Logic;
using UniRx;
using UnityEngine;

namespace GameScene.Models
{
	public interface ITrainObjectModel
	{
		int Id { get; }
		string Name { get; }
		float Speed { get; }
		float Mining { get; }
		IReadOnlyReactiveProperty<Vector2> Position { get; }
		TrainState State { get; }
		int DestinationId { get; }
	}
}