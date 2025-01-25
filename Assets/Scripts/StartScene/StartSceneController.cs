using System;
using System.Linq;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace StartScene
{
	[DisallowMultipleComponent]
	public sealed class StartSceneController : MonoBehaviour
	{
		[SerializeField] private TMP_Dropdown _selectLevelDropdown;

		[Inject] private readonly ZenjectSceneLoader _sceneLoader;
		[Inject] private readonly ILevelsProvider _levelsProvider;

		private void Start()
		{
			_selectLevelDropdown.options = _levelsProvider.Levels
				.Select(model => new TMP_Dropdown.OptionData { text = model.Name }).ToList();
		}

		public void OnPlay()
		{
			throw new NotImplementedException();
		}

		public void OnEditLevel()
		{
			throw new NotImplementedException();
		}

		public void OnNewLevel()
		{
			_sceneLoader.LoadSceneAsync(Const.EditorSceneName);
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_selectLevelDropdown, "_selectLevelDropdown != null");
		}
	}
}