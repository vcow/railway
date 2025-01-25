using UnityEngine;
using Zenject;

namespace StartScene
{
	[DisallowMultipleComponent]
	public sealed class StartSceneInstaller : MonoInstaller<StartSceneInstaller>
	{
		public override void InstallBindings()
		{
		}
	}
}