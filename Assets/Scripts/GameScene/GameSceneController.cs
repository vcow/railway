using GameScene.Logic;
using UnityEngine;
using Zenject;

namespace GameScene
{
	[DisallowMultipleComponent]
	public sealed class GameSceneController : MonoBehaviour
	{
		[Inject] private readonly ZenjectSceneLoader _sceneLoader;
		[Inject] private readonly GameLogic _gameLogic;

		private void Start()
		{
		}
	}
}