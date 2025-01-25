using UnityEngine;
using Zenject;

namespace PreloaderScene
{
	[DisallowMultipleComponent]
	public sealed class PreloaderSceneInstaller : MonoInstaller<PreloaderSceneInstaller>
	{
		public override void InstallBindings()
		{
		}
	}
}