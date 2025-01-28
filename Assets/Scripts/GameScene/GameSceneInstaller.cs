using GameScene.Controllers;
using GameScene.Logic;
using GameScene.Models;
using GameScene.Signals;
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
			Container.BindInterfacesAndSelfTo<GameLogic>().FromNew().AsSingle();

			Container.DeclareSignal<ConnectionLengthChangedSignal>();
			Container.DeclareSignal<VertexMultiplierChangedSignal>();
			Container.DeclareSignal<TrainMiningChangedSignal>();
			Container.DeclareSignal<TrainSpeedChangedSignal>();
		}
	}
}