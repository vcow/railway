using Models;
using UnityEngine;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class GameSceneView : MonoBehaviour
	{
		[Header("Prefabs"), SerializeField] private MineView _minePrefab;

		[Inject] private readonly ILevelModel _levelModel;
	}
}