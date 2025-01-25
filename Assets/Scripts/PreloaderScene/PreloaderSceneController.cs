using Settings;
using UnityEngine;
using Zenject;

namespace PreloaderScene
{
	[DisallowMultipleComponent]
	public sealed class PreloaderSceneController : MonoBehaviour
	{
		[Inject] private readonly ZenjectSceneLoader _sceneLoader;
		[Inject] private readonly LevelsProvider _levelsProvider;

		private async void Start()
		{
			await _levelsProvider.Initialize();
			// TODO: Any other initialize actions here

			_sceneLoader.LoadSceneAsync(Const.StartSceneName);
		}
	}
}