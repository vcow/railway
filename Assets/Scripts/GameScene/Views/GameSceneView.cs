using System.Linq;
using GameScene.Models;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class GameSceneView : MonoBehaviour
	{
		[Header("Settings"), SerializeField] private float _distanceAspect = 1f;
		[SerializeField] private CinemachineTargetGroup _cinemachineTargetGroup;
		[SerializeField] private SpriteRenderer _ground;
		[SerializeField] private Canvas _labelsCanvas;
		[Header("Prefabs"), SerializeField] private MineView _minePrefab;
		[SerializeField] private BaseView _basePrefab;
		[SerializeField] private TrainView _trainPrefab;
		[SerializeField] private ConnectionView _connectionPrefab;

		[Inject] private readonly DiContainer _container;
		[Inject] private readonly IGameModel _gameModel;

		public Bounds Bounds { get; private set; }

		private void Start()
		{
			BuildScene();
			SpawnTrains();

			transform.position = new Vector3(-Bounds.center.x, 0, -Bounds.center.z);
			_cinemachineTargetGroup.DoUpdate();

			if (_ground)
			{
				_ground.size = new Vector2(Bounds.size.x + 4f, Bounds.size.z + 4f);
			}
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_minePrefab, "_minePrefab != null");
			Assert.IsNotNull(_basePrefab, "_basePrefab != null");
			Assert.IsNotNull(_trainPrefab, "_trainPrefab != null");
			Assert.IsNotNull(_connectionPrefab, "_connectionPrefab != null");
		}

		private void BuildScene()
		{
			var xMin = float.MaxValue;
			var yMin = float.MaxValue;
			var xMax = float.MinValue;
			var yMax = float.MinValue;
			var root = transform;

			foreach (var mineModel in _gameModel.Mines)
			{
				var view = _container.InstantiatePrefabForComponent<MineView>(_minePrefab, root,
					new object[] { mineModel, _labelsCanvas });
				view.gameObject.name = $"mine_{mineModel.Id}";
				view.transform.position = new Vector3(mineModel.Position.x, 0f, mineModel.Position.y) * _distanceAspect;
				ApplyToBounds(mineModel.Position);

				if (_cinemachineTargetGroup)
				{
					_cinemachineTargetGroup.AddMember(view.transform, 1f, 1f);
				}
			}

			foreach (var baseModel in _gameModel.Bases)
			{
				var view = _container.InstantiatePrefabForComponent<BaseView>(_basePrefab, root,
					new object[] { baseModel, _labelsCanvas });
				view.gameObject.name = $"base_{baseModel.Id}";
				view.transform.position = new Vector3(baseModel.Position.x, 0f, baseModel.Position.y) * _distanceAspect;
				ApplyToBounds(baseModel.Position);

				if (_cinemachineTargetGroup)
				{
					_cinemachineTargetGroup.AddMember(view.transform, 1f, 1f);
				}
			}

			foreach (var nodeModel in _gameModel.Nodes)
			{
				var view = _container.InstantiateComponentOnNewGameObject<NodeView>($"node_{nodeModel.Id}",
					new object[] { nodeModel });
				var t = view.transform;
				t.SetParent(root);
				t.position = new Vector3(nodeModel.Position.x, 0f, nodeModel.Position.y) * _distanceAspect;
				ApplyToBounds(nodeModel.Position);

				if (_cinemachineTargetGroup)
				{
					_cinemachineTargetGroup.AddMember(view.transform, 1f, 1f);
				}
			}

			Bounds = new Bounds
			{
				min = new Vector3(xMin * _distanceAspect - 1f, 0f, yMin * _distanceAspect - 1f),
				max = new Vector3(xMax * _distanceAspect + 1f, 2f, yMax * _distanceAspect + 1)
			};

			var vertices = _gameModel.GetAllVertices()
				.SelectMany(vertices => vertices).ToDictionary(vertex => vertex.Id);
			foreach (var connectionModel in _gameModel.Connections)
			{
				var from = vertices[connectionModel.FromNodeId].Position;
				var to = vertices[connectionModel.ToNodeId].Position;
				var delta = to - from;
				var ang = Mathf.Atan2(delta.y, delta.x);
				var view = _container.InstantiatePrefabForComponent<ConnectionView>(_connectionPrefab, root,
					new object[] { connectionModel, Quaternion.Euler(-90f, -ang * Mathf.Rad2Deg, 0f), delta.magnitude });
				view.gameObject.name = $"connection_{connectionModel.FromNodeId}-{connectionModel.ToNodeId}";
				var position = from + delta * 0.5f;
				view.transform.position = new Vector3(position.x, 0f, position.y);
			}

			return;

			void ApplyToBounds(Vector2 position)
			{
				xMin = Mathf.Min(xMin, position.x);
				yMin = Mathf.Min(yMin, position.y);
				xMax = Mathf.Max(xMax, position.x);
				yMax = Mathf.Max(yMax, position.y);
			}
		}

		private void SpawnTrains()
		{
			var root = transform;
			foreach (var trainModel in _gameModel.Trains)
			{
				var view = _container.InstantiatePrefabForComponent<TrainView>(_trainPrefab, root,
					new object[] { trainModel, _labelsCanvas });
				view.gameObject.name = $"train_{trainModel.Id}";
			}
		}
	}
}