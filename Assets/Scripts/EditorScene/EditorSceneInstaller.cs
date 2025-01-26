using EditorScene.Signals;
using UnityEngine;
using Zenject;

namespace EditorScene
{
	[DisallowMultipleComponent]
	public sealed class EditorSceneInstaller : MonoInstaller<EditorSceneInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<SelectMarkerSignal>();
			Container.DeclareSignal<DragMarkerSignal>();
			Container.DeclareSignal<ChangeMarkerMultiplierSignal>();
			Container.DeclareSignal<MoveMarkerSignal>();
			Container.DeclareSignal<RenameMarkerSignal>();
			Container.DeclareSignal<SelectConnectionSignal>();
			Container.DeclareSignal<MarkForConnectionSignal>();
			Container.DeclareSignal<RemoveMarkerSignal>();
			Container.DeclareSignal<RemoveConnectionSignal>();
			Container.DeclareSignal<ConnectionChangedSignal>();
			Container.DeclareSignal<SetConnectionLengthSignal>();
		}
	}
}