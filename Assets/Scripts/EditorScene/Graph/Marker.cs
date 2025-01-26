using EditorScene.Signals;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Zenject;

namespace EditorScene.Graph
{
	[DisallowMultipleComponent]
	public abstract class Marker : MonoBehaviour
	{
		[SerializeField] private Image _selector;
		[SerializeField] private Color _selectorColor = Color.white;
		[SerializeField] private Color _connectedSelectorColor = Color.red;

		// ReSharper disable once InconsistentNaming
		[Inject] protected readonly SignalBus _signalBus;
		[Inject] private readonly float _connectionLenMultiplier;

		private RectTransform _rectTransform;
		private bool _markedForConnection;

		private static int _idCtr;

		private void Awake()
		{
			_rectTransform = (RectTransform)transform;
		}

		protected virtual void Start()
		{
			MarkerName = GenerateDefaultName();

			_selector.color = _selectorColor;

			_signalBus.Subscribe<SelectMarkerSignal>(OnSelectMarker);
			_signalBus.Subscribe<MoveMarkerSignal>(OnMoveMarker);
			_signalBus.Subscribe<MarkForConnectionSignal>(OnMarkForConnection);

			_signalBus.TryFire(new SelectMarkerSignal(this));
		}

		protected abstract string GenerateDefaultName();

		protected virtual void OnDestroy()
		{
			_signalBus.Unsubscribe<SelectMarkerSignal>(OnSelectMarker);
			_signalBus.Unsubscribe<MoveMarkerSignal>(OnMoveMarker);
			_signalBus.Unsubscribe<MarkForConnectionSignal>(OnMarkForConnection);

			_signalBus.TryFire(new RemoveMarkerSignal(this));
		}

		protected virtual void OnValidate()
		{
			Assert.IsNotNull(_selector, "_selector != null");
		}

		private void OnSelectMarker(SelectMarkerSignal signal)
		{
			_selector.gameObject.SetActive(signal.Marker == this);
			if (_markedForConnection)
			{
				_markedForConnection = false;
				_selector.color = _selectorColor;
			}
		}

		private void OnMoveMarker(MoveMarkerSignal signal)
		{
			if (signal.Marker == this)
			{
				_rectTransform.anchoredPosition = signal.NewPosition / _connectionLenMultiplier;
			}
		}

		private void OnMarkForConnection(MarkForConnectionSignal signal)
		{
			_markedForConnection = signal.Marker == this;
			_selector.color = _markedForConnection ? _connectedSelectorColor : _selectorColor;
		}

		public virtual void OnPointerDown(BaseEventData eventData)
		{
			_signalBus.TryFire(new SelectMarkerSignal(this));
		}

		public virtual void OnBeginDrag(BaseEventData eventData)
		{
			var pointerEventData = (ExtendedPointerEventData)eventData;
			transform.position = pointerEventData.position;
			_signalBus.TryFire(new DragMarkerSignal(this));
		}

		public virtual void OnDrag(BaseEventData eventData)
		{
			var pointerEventData = (ExtendedPointerEventData)eventData;
			transform.position = pointerEventData.position;
			_signalBus.TryFire(new DragMarkerSignal(this));
		}

		public virtual void OnEndDrag(BaseEventData eventData)
		{
			var pointerEventData = (ExtendedPointerEventData)eventData;
			transform.position = pointerEventData.position;
			_signalBus.TryFire(new DragMarkerSignal(this));
		}

		public int Id { get; } = _idCtr++;

		public Vector2 Position => _rectTransform.anchoredPosition * _connectionLenMultiplier;

		public string MarkerName { get; private set; }
	}
}