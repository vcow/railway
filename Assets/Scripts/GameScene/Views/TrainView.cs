using GameScene.Models;
using GameScene.Signals;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class TrainView : MonoBehaviour
	{
		private readonly CompositeDisposable _disposables = new();

		[SerializeField] private TrainLabelView _labelPrefab;
		[Header("Settings"), SerializeField] private float _speed;
		[SerializeField] private float _mining;

		[Inject] private readonly DiContainer _container;
		[Inject] private readonly ITrainObjectModel _model;
		[Inject] private readonly SignalBus _signalBus;
		[InjectOptional] private Canvas _labelCanvas;

		private void Start()
		{
			if (_labelCanvas)
			{
				_container.InstantiatePrefabForComponent<TrainLabelView>(_labelPrefab, _labelCanvas.transform,
					new object[] { _model, transform });
			}

			_speed = _model.Speed;
			_mining = _model.Mining;

			_model.Position.Subscribe(OnPositionChanged).AddTo(_disposables);

#if UNITY_EDITOR
			this.ObserveEveryValueChanged(view => view._speed)
				.Skip(1)
				.Subscribe(f => _signalBus.TryFire(new TrainSpeedChangedSignal(_model.Id, f)))
				.AddTo(_disposables);
			this.ObserveEveryValueChanged(view => view._mining)
				.Skip(1)
				.Subscribe(f => _signalBus.TryFire(new TrainMiningChangedSignal(_model.Id, f)))
				.AddTo(_disposables);
#endif
		}

		private void OnPositionChanged(Vector2 newPosition)
		{
			transform.localPosition = new Vector3(newPosition.x, 0f, newPosition.y);
		}

		private void OnDestroy()
		{
			_disposables.Dispose();
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_labelPrefab, "_labelPrefab != null");
		}
	}
}