using System.Collections.Generic;
using EditorScene.Builders;
using EditorScene.Signals;
using Models;
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
		[Inject] private readonly SignalBus _signalBus;
		[Inject] private readonly LevelModelBuilder _levelModelBuilder;

		private TrainsListItem _selectedItem;

		private void Start()
		{
			foreach (var trainModel in _levelModelBuilder.LevelModel.Trains)
			{
				AddItem(trainModel.Name, trainModel.Speed, trainModel.Mining);
			}
		}

		public void OnAdd()
		{
			var speed = _speedInput.text.ToFloat();
			var mining = _miningInput.text.ToFloat();
			if (speed <= 0f || mining <= 0f)
			{
				return;
			}

			AddItem(_nameInput.text, speed, mining);
		}

		private void AddItem(string trainName, float speed, float mining)
		{
			var index = _listContainer.childCount;
			var item = _container.InstantiatePrefabForComponent<TrainsListItem>(_listItemPrefab, _listContainer,
				new object[]
				{
					index,
					trainName,
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
			var trains = new List<TrainModelImpl>(_listContainer.childCount);
			foreach (Transform child in _listContainer)
			{
				var item = child.GetComponent<TrainsListItem>();
				trains.Add(new TrainModelImpl
				{
					Name = item.TrainName,
					Speed = item.Speed,
					Mining = item.Mining
				});
			}

			_signalBus.TryFire(new TrainsListChangedSignal(trains));
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