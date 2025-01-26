using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;
using Assert = UnityEngine.Assertions.Assert;

namespace EditorScene.Graph.TrainsList
{
	[DisallowMultipleComponent]
	public sealed class TrainsListWindow : MonoBehaviour
	{
		[Header("List"), SerializeField] private TrainsListItem _listItemPrefab;
		[SerializeField] private Transform _listContainer;
		[SerializeField] private ToggleGroup _toggleGroup;
		[Header("Input"), SerializeField] private TMP_InputField _nameInput;
		[SerializeField] private TMP_InputField _speedInput;
		[SerializeField] private TMP_InputField _miningInput;

		[Inject] private readonly DiContainer _container;

		private TrainsListItem _selectedItem;

		public void OnAdd()
		{
			var speed = _speedInput.text.ToFloat();
			var mining = _miningInput.text.ToFloat();
			if (speed <= 0f || mining <= 0f)
			{
				return;
			}

			var index = _listContainer.childCount;
			var item = _container.InstantiatePrefabForComponent<TrainsListItem>(_listItemPrefab, _listContainer,
				new object[]
				{
					index,
					_nameInput.text,
					(speed, mining)
				});
			item.Toggle.group = _toggleGroup;
			item.Toggle.onValueChanged.AddListener(b =>
			{
				if (!b)
				{
					return;
				}

				_selectedItem = item;
			});

			if (item.Toggle.isOn)
			{
				_selectedItem = item;
			}
		}

		public void OnDelete()
		{
			if (_selectedItem == null)
			{
				return;
			}

			var index = 0;
			foreach (Transform child in _listContainer)
			{
				if (child.gameObject == _selectedItem.gameObject)
				{
					continue;
				}

				child.GetComponent<TrainsListItem>().Index = index++;
			}

			Destroy(_selectedItem.gameObject);
			_selectedItem = null;
		}

		public void OnExit()
		{
			gameObject.SetActive(false);
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_listItemPrefab, "_listItemPrefab != null");
			Assert.IsNotNull(_listContainer, "_listContainer != null");
			Assert.IsNotNull(_toggleGroup, "_toggleGroup != null");
			Assert.IsNotNull(_nameInput, "_nameInput != null");
			Assert.IsNotNull(_speedInput, "_speedInput != null");
			Assert.IsNotNull(_miningInput, "_miningInput != null");
		}
	}
}