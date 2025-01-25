using UnityEngine;
using Zenject;

namespace EditorScene.Graph
{
	[DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
	public class GraphView : MonoInstaller<GraphView>
	{
		public override void InstallBindings()
		{
			Container.Bind<GraphView>().FromInstance(this).AsSingle();
		}

		public void AddMine()
		{
			
		}

		public void AddBase()
		{
			
		}

		public void AddConnection()
		{
			
		}

		public void RemoveConnection()
		{
			
		}
	}
}