using GameScene.Controllers;
using GameScene.Models;
using UnityEngine;
using Zenject;

namespace GameScene
{
	[DisallowMultipleComponent]
	public sealed class GameSceneInstaller : MonoInstaller<GameSceneInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<GameModelController>().FromNew().AsSingle();
			Container.Bind<IGameModel>().FromResolveGetter<GameModelController>(controller => controller.GameModel).AsSingle();
		}
	}
}