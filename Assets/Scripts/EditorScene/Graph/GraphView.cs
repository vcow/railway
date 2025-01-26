using System;
using Cysharp.Threading.Tasks;
using EditorScene.Builders;
using EditorScene.Signals;
using Models;
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
		[Inject] private readonly LevelModelBuilder _levelModelBuilder;

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

			RestoreLevel(_levelModelBuilder.LevelModel);
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<SelectMarkerSignal>(OnSelectMarker);
			_signalBus.Unsubscribe<SelectConnectionSignal>(OnSelectConnection);
		}

		private void RestoreLevel(ILevelModel levelModel)
		{
			foreach (var nodeModel in levelModel.Nodes)
			{
				switch (nodeModel.Type)
				{
					case NodeType.Node:
						AddNode(nodeModel);
						break;
					case NodeType.Mine:
						AddMine(nodeModel);
						break;
					case NodeType.Base:
						AddBase(nodeModel);
						break;
					default:
						throw new NotSupportedException();
				}
			}

			foreach (var connectionModel in levelModel.Connections)
			{
				var from = FindMarker(connectionModel.FromNodeId);
				var to = FindMarker(connectionModel.ToNodeId);
				if (!from || !to)
				{
					Debug.LogError($"Can't restore connection {connectionModel.FromNodeId}-{connectionModel.ToNodeId}.");
					continue;
				}

				CreateConnection(from, to, connectionModel.Length);
			}
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

			var connection = CreateConnection(_selectedMarker, signal.Marker, null);

			await UniTask.Yield(PlayerLoopTiming.Update);

			_signalBus.TryFire(new SelectConnectionSignal(connection));
		}

		private NodeConnection CreateConnection(Marker from, Marker to, float? length)
		{
			var connection = _container.InstantiatePrefabForComponent<NodeConnection>(_nodeConnectionPrefab, _connectionsLayer,
				length.HasValue
					? new object[] { (from, to), _connectionLenMultiplier, length }
					: new object[] { (from, to), _connectionLenMultiplier });
			connection.gameObject.name = $"connection_{from.Id}-{to.Id}";
			return connection;
		}

		private Marker FindMarker(int id)
		{
			foreach (Transform child in _markersLayer)
			{
				var marker = child.GetComponent<Marker>();
				if (!marker)
				{
					continue;
				}

				if (marker.Id == id)
				{
					return marker;
				}
			}

			return null;
		}

		private void OnSelectConnection(SelectConnectionSignal signal)
		{
			_selectedConnection = signal.NodeConnection;
			if (_selectedConnection != null)
			{
				_signalBus.TryFire(new SelectMarkerSignal(null));
			}
		}

		public void AddMine(INodeModel nodeModel = null)
		{
			Assert.IsTrue(nodeModel == null || nodeModel.Type == NodeType.Mine);

			_isConnection = false;
			var marker = _container.InstantiatePrefabForComponent<Marker>(_mineMarkerPrefab, _markersLayer,
				nodeModel != null
					? new object[] { _connectionLenMultiplier, nodeModel }
					: new object[] { _connectionLenMultiplier });
			marker.gameObject.name = $"mine_{marker.Id}";
			if (nodeModel == null)
			{
				marker.transform.position = new Vector2(Screen.width, Screen.height) * 0.5f;
			}
		}

		public void AddBase(INodeModel nodeModel = null)
		{
			Assert.IsTrue(nodeModel == null || nodeModel.Type == NodeType.Base);

			_isConnection = false;
			var marker = _container.InstantiatePrefabForComponent<Marker>(_baseMarkerPrefab, _markersLayer,
				nodeModel != null
					? new object[] { _connectionLenMultiplier, nodeModel }
					: new object[] { _connectionLenMultiplier });
			marker.gameObject.name = $"base_{marker.Id}";
			if (nodeModel == null)
			{
				marker.transform.position = new Vector2(Screen.width, Screen.height) * 0.5f;
			}
		}

		public void AddNode(INodeModel nodeModel = null)
		{
			Assert.IsTrue(nodeModel == null || nodeModel.Type == NodeType.Node);

			_isConnection = false;
			var marker = _container.InstantiatePrefabForComponent<Marker>(_nodeMarkerPrefab, _markersLayer,
				nodeModel != null
					? new object[] { _connectionLenMultiplier, nodeModel }
					: new object[] { _connectionLenMultiplier });
			marker.gameObject.name = $"node_{marker.Id}";
			if (nodeModel == null)
			{
				marker.transform.position = new Vector2(Screen.width, Screen.height) * 0.5f;
			}
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
				_signalBus.TryFire(new SelectMarkerSignal(null));
			}
			else if (_selectedConnection != null)
			{
				Destroy(_selectedConnection.gameObject);
				_signalBus.TryFire(new SelectConnectionSignal(null));
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