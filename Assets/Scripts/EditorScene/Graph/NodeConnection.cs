using Cysharp.Threading.Tasks;
using EditorScene.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace EditorScene.Graph
{
	[DisallowMultipleComponent]
	public sealed class NodeConnection : MonoBehaviour
	{
		[SerializeField] private Image _line;
		[SerializeField] private TextMeshProUGUI _lengthLabel;
		[SerializeField] private Color _lineColor;
		[SerializeField] private Color _selectedLineColor;

		[Inject] private readonly SignalBus _signalBus;
		[Inject] private readonly (Marker from, Marker to) _connectedMarkers;
		[Inject] private readonly float _connectionLenMultiplier;
		[InjectOptional] private readonly float? _length;

		private void Start()
		{
			_signalBus.Subscribe<SelectConnectionSignal>(OnSelectConnection);
			_signalBus.Subscribe<DragMarkerSignal>(OnDragMarker);
			_signalBus.Subscribe<MoveMarkerSignal>(OnMoveMarker);
			_signalBus.Subscribe<RemoveMarkerSignal>(OnRemoveMarker);
			_signalBus.Subscribe<SetConnectionLengthSignal>(OnSetConnectionLength);
			_signalBus.Subscribe<SetConnectionDistanceSignal>(OnSetConnectionDistance);

			UpdateConnection();

			_signalBus.TryFire(new SetConnectionLengthSignal(this, _length ?? Distance));
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<SelectConnectionSignal>(OnSelectConnection);
			_signalBus.Unsubscribe<DragMarkerSignal>(OnDragMarker);
			_signalBus.Unsubscribe<MoveMarkerSignal>(OnMoveMarker);
			_signalBus.Unsubscribe<RemoveMarkerSignal>(OnRemoveMarker);
			_signalBus.Unsubscribe<SetConnectionLengthSignal>(OnSetConnectionLength);
			_signalBus.Unsubscribe<SetConnectionDistanceSignal>(OnSetConnectionDistance);

			_signalBus.TryFire(new RemoveConnectionSignal(this));
		}

		private void OnSelectConnection(SelectConnectionSignal signal)
		{
			_line.color = signal.NodeConnection == this ? _selectedLineColor : _lineColor;
		}

		private async void OnDragMarker(DragMarkerSignal signal)
		{
			if (_connectedMarkers.from != signal.Marker && _connectedMarkers.to != signal.Marker)
			{
				return;
			}

			await UniTask.Yield(PlayerLoopTiming.Update);
			UpdateConnection();
		}

		private async void OnMoveMarker(MoveMarkerSignal signal)
		{
			if (_connectedMarkers.from != signal.Marker && _connectedMarkers.to != signal.Marker)
			{
				return;
			}

			await UniTask.Yield(PlayerLoopTiming.Update);
			UpdateConnection();
		}

		private void OnRemoveMarker(RemoveMarkerSignal signal)
		{
			if (_connectedMarkers.from != signal.Marker && _connectedMarkers.to != signal.Marker)
			{
				return;
			}

			Destroy(gameObject);
		}

		private void OnSetConnectionLength(SetConnectionLengthSignal signal)
		{
			if (signal.NodeConnection != this)
			{
				return;
			}

			Length = signal.Length;
			_lengthLabel.text = $"({Distance}) {Length}";
		}

		private void OnSetConnectionDistance(SetConnectionDistanceSignal signal)
		{
			if (signal.NodeConnection != this)
			{
				return;
			}

			var fromPosition = _connectedMarkers.from.Position;
			var toPosition = _connectedMarkers.to.Position;
			var delta = toPosition - fromPosition;
			var ang = Mathf.Atan2(delta.y, delta.x);
			var relToPositionMarker = new Vector2(Mathf.Cos(ang) * signal.Distance, Mathf.Sin(ang) * signal.Distance);
			_signalBus.TryFire(new MoveMarkerSignal(_connectedMarkers.to, fromPosition + relToPositionMarker));
		}

		public void OnPointerClick(BaseEventData eventData)
		{
			_signalBus.TryFire(new SelectConnectionSignal(this));
		}

		private void UpdateConnection()
		{
			var fromScenePosition = _connectedMarkers.from.transform.position;
			var toScenePosition = _connectedMarkers.to.transform.position;
			var connectionBounds = new Bounds
			{
				min = new Vector3(Mathf.Min(fromScenePosition.x, toScenePosition.x),
					Mathf.Min(fromScenePosition.y, toScenePosition.y),
					Mathf.Min(fromScenePosition.z, toScenePosition.z)),
				max = new Vector3(Mathf.Max(fromScenePosition.x, toScenePosition.x),
					Mathf.Max(fromScenePosition.y, toScenePosition.y),
					Mathf.Max(fromScenePosition.z, toScenePosition.z))
			};

			transform.position = connectionBounds.center;

			var fromPosition = _connectedMarkers.from.Position;
			var toPosition = _connectedMarkers.to.Position;
			var delta = toPosition - fromPosition;
			var len = delta.magnitude;
			_line.rectTransform.sizeDelta = new Vector2(len / _connectionLenMultiplier, _line.rectTransform.sizeDelta.y);
			var ang = Mathf.Atan2(delta.y, delta.x);
			_line.transform.rotation = Quaternion.Euler(0f, 0f, ang * Mathf.Rad2Deg);

			Distance = Mathf.Round(len);
			_lengthLabel.text = $"({Distance}) {Length}";
			_signalBus.TryFire(new ConnectionChangedSignal(this));
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_line, "_line != null");
			Assert.IsNotNull(_lengthLabel, "_lengthLabel != null");
		}

		public float Length { get; private set; }

		public float Distance { get; private set; }

		public Marker From => _connectedMarkers.from;

		public Marker To => _connectedMarkers.to;
	}
}