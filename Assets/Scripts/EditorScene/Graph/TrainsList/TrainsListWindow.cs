using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

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

		public void OnAdd()
		{
		}

		public void OnDelete()
		{
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