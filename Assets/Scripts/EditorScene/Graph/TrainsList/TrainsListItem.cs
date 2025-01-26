using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
using Toggle = UnityEngine.UIElements.Toggle;

namespace EditorScene.Graph.TrainsList
{
	[DisallowMultipleComponent, RequireComponent(typeof(Toggle))]
	public sealed class TrainsListItem : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _idLabel;
		[SerializeField] private TextMeshProUGUI _nameLabel;
		[SerializeField] private TextMeshProUGUI _speedLabel;
		[SerializeField] private TextMeshProUGUI _miningLabel;

		[Inject] private readonly int _index;
		[Inject] private readonly string _name;
		[Inject] private readonly (float speed, float mining) _characteristics;

		public Toggle Toggle => GetComponent<Toggle>();

		private void Start()
		{
			_idLabel.text = (_index + 1).ToString();
			_nameLabel.text = _name;
			_speedLabel.text = _characteristics.speed.ToString(CultureInfo.InvariantCulture);
			_miningLabel.text = _characteristics.mining.ToString(CultureInfo.InvariantCulture);
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_idLabel, "_idLabel != null");
			Assert.IsNotNull(_nameLabel, "_nameLabel != null");
			Assert.IsNotNull(_speedLabel, "_speedLabel != null");
			Assert.IsNotNull(_miningLabel, "_miningLabel != null");
		}
	}
}