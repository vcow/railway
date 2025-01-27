using GameScene.Models;
using GameScene.Signals;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class BaseView : MonoBehaviour
	{
		private readonly CompositeDisposable _disposables = new();

		[SerializeField] private BaseLabelView _labelPrefab;
		[Header("Settings"), SerializeField] private float _multiplier;

		[Inject] private readonly DiContainer _container;
		[Inject] private readonly IBaseVertexModel _model;
		[Inject] private readonly SignalBus _signalBus;
		[InjectOptional] private Canvas _labelCanvas;

		private void Start()
		{
			if (_labelCanvas)
			{
				_container.InstantiatePrefabForComponent<BaseLabelView>(_labelPrefab, _labelCanvas.transform,
					new object[] { _model, transform });
			}

			_multiplier = _model.Multiplier;
#if UNITY_EDITOR
			this.ObserveEveryValueChanged(view => view._multiplier)
				.Skip(1)
				.Subscribe(f => _signalBus.TryFire(new VertexMultiplierChangedSignal(_model.Id, f)))
				.AddTo(_disposables);
#endif
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