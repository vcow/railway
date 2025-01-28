using GameScene.Models;
using GameScene.Signals;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class MineView : MonoBehaviour
	{
		private readonly CompositeDisposable _disposables = new();

		[SerializeField] private MineLabelView _labelPrefab;
		[Header("Settings"), SerializeField] private float _multiplier;

		[Inject] private readonly DiContainer _container;
		[Inject] private readonly IMineVertexModel _model;
		[Inject] private readonly SignalBus _signalBus;
		[InjectOptional] private Canvas _labelCanvas;

		private void Start()
		{
			if (_labelCanvas)
			{
				_container.InstantiatePrefabForComponent<MineLabelView>(_labelPrefab, _labelCanvas.transform,
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

		private void OnDrawGizmos()
		{
			Handles.Label(transform.position, _model.Id.ToString());
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_labelPrefab, "_labelPrefab != null");
		}
	}
}