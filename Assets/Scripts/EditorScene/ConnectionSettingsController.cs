using System.Globalization;
using EditorScene.Graph;
using EditorScene.Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace EditorScene
{
	[DisallowMultipleComponent]
	public sealed class ConnectionSettingsController : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _lengthInput;

		[Inject] private readonly SignalBus _signalBus;

		private NodeConnection _selectedConnection;

		private void Start()
		{
			_signalBus.Subscribe<ConnectionChangedSignal>(OnConnectionChanged);
			_signalBus.Subscribe<SelectConnectionSignal>(OnSelectConnection);

			gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<ConnectionChangedSignal>(OnConnectionChanged);
			_signalBus.Unsubscribe<SelectConnectionSignal>(OnSelectConnection);
		}

		public void OnApply()
		{
			var newValue = float.Parse(_lengthInput.text);
			if (_selectedConnection == null || _selectedConnection.Length.Equals(newValue))
			{
				return;
			}

			_signalBus.TryFire(new SetConnectionLengthSignal(_selectedConnection, newValue));
		}

		private void OnConnectionChanged(ConnectionChangedSignal signal)
		{
			_lengthInput.text = signal.NodeConnection.Length.ToString(CultureInfo.InvariantCulture);
		}

		private void OnSelectConnection(SelectConnectionSignal signal)
		{
			_selectedConnection = signal.NodeConnection;
			if (_selectedConnection == null)
			{
				gameObject.SetActive(false);
				return;
			}

			_lengthInput.text = _selectedConnection.Length.ToString(CultureInfo.InvariantCulture);
			gameObject.SetActive(true);
		}
	}
}