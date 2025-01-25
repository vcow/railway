using UnityEngine;
using Zenject;

namespace EditorScene
{
	[DisallowMultipleComponent]
	public sealed class EditorSceneInstaller : MonoInstaller<EditorSceneInstaller>
	{
		public override void InstallBindings()
		{
		}
	}
}