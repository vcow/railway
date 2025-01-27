using GameScene.Models;
using GameScene.Signals;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class ConnectionView : MonoBehaviour
	{
		private readonly CompositeDisposable _disposables = new();

		[SerializeField] private SpriteRenderer _view;
		[SerializeField] private float _width;
		[Header("Settings"), SerializeField] private float _length;

		[Inject] private readonly Quaternion _rotation;
		[Inject] private readonly float _distance;
		[Inject] private readonly IConnectionEdgeModel _model;
		[Inject] private readonly SignalBus _signalBus;

		private void Start()
		{
			_view.transform.rotation = _rotation;
			_view.size = new Vector2(_distance, _width);

			_length = _model.Length;
#if UNITY_EDITOR
			this.ObserveEveryValueChanged(view => view._length)
				.Skip(1)
				.Subscribe(f => _signalBus.TryFire(new ConnectionLengthChangedSignal(_model.FromNodeId, _model.ToNodeId, f)))
				.AddTo(_disposables);
#endif
		}

		private void OnDestroy()
		{
			_disposables.Dispose();
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_view, "_view != null");
		}
	}
}