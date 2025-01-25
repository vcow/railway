using Cysharp.Threading.Tasks;
using EditorScene.Signals;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace EditorScene.Graph
{
	[DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
	public class GraphView : MonoInstaller<GraphView>
	{
		[Header("Settings"), SerializeField] private float _connectionLenMultiplier = 1f;
		[Header("Prefabs"), SerializeField] private Marker _baseMarkerPrefab;
		[SerializeField] private Marker _mineMarkerPrefab;
		[SerializeField] private Marker _nodeMarkerPrefab;
		[Space, SerializeField] private NodeConnection _nodeConnectionPrefab;
		[Header("Layers"), SerializeField] private RectTransform _connectionsLayer;
		[SerializeField] private RectTransform _markersLayer;

		[Inject] private readonly DiContainer _container;
		[Inject] private readonly SignalBus _signalBus;

		private Marker _selectedMarker;
		private NodeConnection _selectedConnection;
		private bool _isConnection;

		public override void InstallBindings()
		{
			Container.Bind<GraphView>().FromInstance(this).AsSingle();
		}

		public override void Start()
		{
			_signalBus.Subscribe<SelectMarkerSignal>(OnSelectMarker);
			_signalBus.Subscribe<SelectConnectionSignal>(OnSelectConnection);
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<SelectMarkerSignal>(OnSelectMarker);
			_signalBus.Unsubscribe<SelectConnectionSignal>(OnSelectConnection);
		}

		private async void OnSelectMarker(SelectMarkerSignal signal)
		{
			if (!_isConnection || _selectedMarker == null)
			{
				if (signal.Marker != null)
				{
					_signalBus.TryFire(new SelectConnectionSignal(null));
				}

				_selectedMarker = signal.Marker;
				return;
			}

			_isConnection = false;
			if (_selectedMarker == signal.Marker)
			{
				// Connect to himself
				return;
			}

			var from = _selectedMarker;
			var to = signal.Marker;
			var connection = _container.InstantiatePrefabForComponent<NodeConnection>(_nodeConnectionPrefab, _connectionsLayer,
				new object[] { (from, to), _connectionLenMultiplier });
			connection.gameObject.name = $"connection_{from.Id}-{to.Id}";

			await UniTask.Yield(PlayerLoopTiming.Update);

			_signalBus.TryFire(new SelectConnectionSignal(connection));
		}

		private void OnSelectConnection(SelectConnectionSignal signal)
		{
			_selectedConnection = signal.NodeConnection;
			if (_selectedConnection != null)
			{
				_signalBus.TryFire(new SelectMarkerSignal(null));
			}
		}

		public void AddMine()
		{
			_isConnection = false;
			var marker = _container.InstantiatePrefabForComponent<Marker>(_mineMarkerPrefab, _markersLayer);
			marker.gameObject.name = $"mine_{marker.Id}";
			marker.transform.position = new Vector2(Screen.width, Screen.height) * 0.5f;
		}

		public void AddBase()
		{
			_isConnection = false;
			var marker = _container.InstantiatePrefabForComponent<Marker>(_baseMarkerPrefab, _markersLayer);
			marker.gameObject.name = $"base_{marker.Id}";
			marker.transform.position = new Vector2(Screen.width, Screen.height) * 0.5f;
		}

		public void AddNode()
		{
			_isConnection = false;
			var marker = _container.InstantiatePrefabForComponent<Marker>(_nodeMarkerPrefab, _markersLayer);
			marker.gameObject.name = $"node_{marker.Id}";
			marker.transform.position = new Vector2(Screen.width, Screen.height) * 0.5f;
		}

		public void AddConnection()
		{
			if (_selectedMarker == null)
			{
				return;
			}

			_isConnection = true;
			_signalBus.TryFire(new MarkForConnectionSignal(_selectedMarker));
		}

		public void RemoveSelected()
		{
			if (_selectedMarker != null)
			{
				_isConnection = false;
				Destroy(_selectedMarker.gameObject);
				_selectedMarker = null;
			}

			if (_selectedConnection != null)
			{
				Destroy(_selectedConnection.gameObject);
				_selectedConnection = null;
			}
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_baseMarkerPrefab, "_baseMarkerPrefab != null");
			Assert.IsNotNull(_mineMarkerPrefab, "_mineMarkerPrefab != null");
			Assert.IsNotNull(_nodeMarkerPrefab, "_nodeMarkerPrefab != null");
			Assert.IsNotNull(_connectionsLayer, "_connectionsLayer != null");
			Assert.IsNotNull(_markersLayer, "_markersLayer != null");
		}
	}
}