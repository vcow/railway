using System;
using System.Globalization;
using EditorScene.Graph;
using EditorScene.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;
using Zenject;

namespace EditorScene
{
	[DisallowMultipleComponent]
	public sealed class MarkerSettingsController : MonoBehaviour
	{
		[SerializeField] private GameObject _nameField;
		[SerializeField] private GameObject _multiplierField;
		[Header("Inputs"), SerializeField] private TMP_InputField _xInput;
		[SerializeField] private TMP_InputField _yInput;
		[SerializeField] private TMP_InputField _nameInput;
		[SerializeField] private TMP_InputField _multiplierInput;

		[Inject] private readonly SignalBus _signalBus;

		private Marker _selectedMarker;

		private void Start()
		{
			_signalBus.Subscribe<SelectMarkerSignal>(OnSelectMarker);
			_signalBus.Subscribe<DragMarkerSignal>(OnDragMarker);

			_xInput.onValueChanged.AddListener(OnChangeXPosition);
			_yInput.onValueChanged.AddListener(OnChangeYPosition);
			_nameInput.onValueChanged.AddListener(OnChangeName);
			_multiplierInput.onValueChanged.AddListener(OnChangeMultiplier);

			gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<SelectMarkerSignal>(OnSelectMarker);
			_signalBus.Unsubscribe<DragMarkerSignal>(OnDragMarker);

			_xInput.onValueChanged.RemoveListener(OnChangeXPosition);
			_yInput.onValueChanged.RemoveListener(OnChangeYPosition);
			_nameInput.onValueChanged.RemoveListener(OnChangeName);
			_multiplierInput.onValueChanged.RemoveListener(OnChangeMultiplier);
		}

		private void OnChangeXPosition(string value)
		{
			var newValue = value.ToFloat();
			if (_selectedMarker == null || _selectedMarker.Position.x.Equals(newValue))
			{
				return;
			}

			_signalBus.TryFire(new MoveMarkerSignal(_selectedMarker, new Vector2(newValue, _selectedMarker.Position.y)));
		}

		private void OnChangeYPosition(string value)
		{
			var newValue = value.ToFloat();
			if (_selectedMarker == null || _selectedMarker.Position.y.Equals(newValue))
			{
				return;
			}

			_signalBus.TryFire(new MoveMarkerSignal(_selectedMarker, new Vector2(_selectedMarker.Position.x, newValue)));
		}

		private void OnChangeName(string value)
		{
			if (_selectedMarker == null || _selectedMarker.MarkerName == value)
			{
				return;
			}

			_signalBus.TryFire(new RenameMarkerSignal(_selectedMarker, value));
		}

		private void OnChangeMultiplier(string value)
		{
			var newValue = value.ToFloat();
			if (_selectedMarker is BaseMarker baseMarker && baseMarker.Multiplier.Equals(newValue) ||
			    _selectedMarker is MineMarker mineMarker && mineMarker.Multiplier.Equals(newValue))
			{
				return;
			}

			_signalBus.TryFire(new ChangeMarkerMultiplierSignal(_selectedMarker, newValue));
		}

		private void OnSelectMarker(SelectMarkerSignal signal)
		{
			_selectedMarker = signal.Marker;
			if (_selectedMarker == null)
			{
				gameObject.SetActive(false);
				return;
			}

			Vector2 position;
			string markerName;
			float multiplier;

			switch (signal.Marker)
			{
				case NodeMarker nodeMarker:
					_nameField.SetActive(false);
					_multiplierField.SetActive(false);
					position = nodeMarker.Position;
					markerName = nodeMarker.MarkerName;
					multiplier = 1f;
					break;
				case BaseMarker baseMarker:
					_nameField.SetActive(true);
					_multiplierField.SetActive(true);
					position = baseMarker.Position;
					markerName = baseMarker.MarkerName;
					multiplier = baseMarker.Multiplier;
					break;
				case MineMarker mineMarker:
					_nameField.SetActive(true);
					_multiplierField.SetActive(true);
					position = mineMarker.Position;
					markerName = mineMarker.MarkerName;
					multiplier = mineMarker.Multiplier;
					break;
				default:
					throw new NotSupportedException();
			}

			_xInput.text = position.x.ToString(CultureInfo.InvariantCulture);
			_yInput.text = position.y.ToString(CultureInfo.InvariantCulture);
			_multiplierInput.text = multiplier.ToString(CultureInfo.InvariantCulture);
			_nameInput.text = markerName;

			gameObject.SetActive(true);
		}

		private void OnDragMarker(DragMarkerSignal signal)
		{
			var position = signal.Marker.Position;
			_xInput.text = position.x.ToString(CultureInfo.InvariantCulture);
			_yInput.text = position.y.ToString(CultureInfo.InvariantCulture);
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_xInput, "_xInput != null");
			Assert.IsNotNull(_yInput, "_yInput != null");
			Assert.IsNotNull(_nameInput, "_nameInput != null");
			Assert.IsNotNull(_multiplierInput, "_multiplierInput != null");
			Assert.IsNotNull(_nameField, "_nameField != null");
			Assert.IsNotNull(_multiplierField, "_multiplierField != null");
		}
	}
}