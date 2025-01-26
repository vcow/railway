using System.IO;
using Cysharp.Threading.Tasks;
using EditorScene.Builders;
using EditorScene.Graph;
using EditorScene.Signals;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace EditorScene
{
	[DisallowMultipleComponent]
	public sealed class EditorSceneController : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _nameInput;

		[Inject] private readonly ZenjectSceneLoader _sceneLoader;
		[Inject] private readonly GraphView _graphView;
		[Inject] private readonly LevelModelBuilder _levelModelBuilder;
		[Inject] private readonly SignalBus _signalBus;

		private void Start()
		{
			_nameInput.text = _levelModelBuilder.LevelModel.Name;
			_nameInput.onValueChanged.AddListener(OnNameChanged);

			FileBrowser.SetFilters(true, new FileBrowser.Filter("Config", ".json"));
			FileBrowser.SetDefaultFilter(".json");
		}

		private void OnDestroy()
		{
			_nameInput.onValueChanged.RemoveListener(OnNameChanged);
		}

		private void OnNameChanged(string value)
		{
			_signalBus.Fire(new SetLevelNameSignal(_nameInput.text));
		}

		public void OnAddMine()
		{
			_graphView.AddMine();
		}

		public void OnAddBase()
		{
			_graphView.AddBase();
		}

		public void OnAddNode()
		{
			_graphView.AddNode();
		}

		public void OnAddConnection()
		{
			_graphView.AddConnection();
		}

		public void OnRemove()
		{
			_graphView.RemoveSelected();
		}

		public async void OnSave()
		{
			await FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files).ToUniTask();
			if (FileBrowser.Success)
			{
				var data = _levelModelBuilder.GetSerializedData();

				var path = FileBrowser.Result[0];
				if (File.Exists(path))
				{
					File.Delete(path);
				}

				await File.WriteAllTextAsync(path, data);
			}
		}

		public void OnExit()
		{
			_sceneLoader.LoadSceneAsync(Const.StartSceneName);
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_nameInput, "_nameInput != null");
		}
	}
}