using UnityEngine;
using Zenject;

namespace GameScene
{
	[DisallowMultipleComponent]
	public sealed class GameSceneController : MonoBehaviour
	{
		[Inject] private readonly ZenjectSceneLoader _sceneLoader;

		private void Start()
		{
		}
	}
}