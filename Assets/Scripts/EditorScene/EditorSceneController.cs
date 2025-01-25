using System;
using EditorScene.Graph;
using UnityEngine;
using Zenject;

namespace EditorScene
{
	[DisallowMultipleComponent]
	public sealed class EditorSceneController : MonoBehaviour
	{
		[Inject] private readonly ZenjectSceneLoader _sceneLoader;
		[Inject] private readonly GraphView _graphView;

		private void Start()
		{
		}

		public void OnAddMine()
		{
			_graphView.AddMine();
		}

		public void OnAddBase()
		{
			_graphView.AddBase();
		}

		public void OnAddConnection()
		{
			_graphView.AddConnection();
		}

		public void OnRemoveConnection()
		{
			_graphView.RemoveConnection();
		}

		public void OnSave()
		{
			throw new NotImplementedException();
		}

		public void OnExit()
		{
			_sceneLoader.LoadSceneAsync(Const.StartSceneName);
		}
	}
}