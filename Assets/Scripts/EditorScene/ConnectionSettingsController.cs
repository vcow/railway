using System.Globalization;
using EditorScene.Graph;
using EditorScene.Signals;
using TMPro;
using UnityEngine;
using Utils;
using Zenject;

namespace EditorScene
{
	[DisallowMultipleComponent]
	public sealed class ConnectionSettingsController : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _lengthInput;
		[SerializeField] private TMP_InputField _distanceInput;

		[Inject] private readonly SignalBus _signalBus;

		private NodeConnection _selectedConnection;

		private void Start()
		{
			_signalBus.Subscribe<ConnectionChangedSignal>(OnConnectionChanged);
			_signalBus.Subscribe<SelectConnectionSignal>(OnSelectConnection);

			_lengthInput.onValueChanged.AddListener(OnChangeLength);

			gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<ConnectionChangedSignal>(OnConnectionChanged);
			_signalBus.Unsubscribe<SelectConnectionSignal>(OnSelectConnection);

			_lengthInput.onValueChanged.RemoveListener(OnChangeLength);
		}

		private void OnChangeLength(string value)
		{
			if (_selectedConnection == null)
			{
				return;
			}

			_signalBus.TryFire(new SetConnectionLengthSignal(_selectedConnection, value.ToFloat()));
		}

		public void OnApply()
		{
			var newValue = _distanceInput.text.ToFloat();
			if (_selectedConnection == null || _selectedConnection.Length.Equals(newValue))
			{
				return;
			}

			_signalBus.TryFire(new SetConnectionDistanceSignal(_selectedConnection, newValue));
		}

		private void OnConnectionChanged(ConnectionChangedSignal signal)
		{
			_distanceInput.text = signal.NodeConnection.Distance.ToString(CultureInfo.InvariantCulture);
		}

		private void OnSelectConnection(SelectConnectionSignal signal)
		{
			_selectedConnection = signal.NodeConnection;
			if (_selectedConnection == null)
			{
				gameObject.SetActive(false);
				return;
			}

			_distanceInput.text = _selectedConnection.Distance.ToString(CultureInfo.InvariantCulture);
			_lengthInput.text = _selectedConnection.Length.ToString(CultureInfo.InvariantCulture);
			gameObject.SetActive(true);
		}
	}
}