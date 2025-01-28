using System;
using GameScene.Models;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Views
{
	public sealed class BaseLabelView : LabelView
	{
		private const float HideCoinDelayTimeSec = 2f;

		private readonly CompositeDisposable _disposables = new();

		[SerializeField] private GameObject _coinIcon;
		[SerializeField] private TextMeshProUGUI _counter;

		[Inject] private readonly IBaseVertexModel _model;

		private float _lastResources;
		private IDisposable _timerHandler;

		protected override void Start()
		{
			base.Start();
			LabelText = _model.Name;
			_coinIcon.SetActive(false);

			_lastResources = _model.Resources.Value;
			_model.Resources.Subscribe(f =>
			{
				ShowNewResources(f - _lastResources);
				_lastResources = f;
			}).AddTo(_disposables);
		}

		private void ShowNewResources(float delta)
		{
			if (Mathf.Abs(delta) < 0.01f)
			{
				return;
			}

			_counter.text = delta.ToString("F2");
			_coinIcon.SetActive(true);

			if (_timerHandler != null)
			{
				_disposables.Remove(_timerHandler);
			}

			_timerHandler = Observable.Timer(TimeSpan.FromSeconds(HideCoinDelayTimeSec), Scheduler.MainThread)
				.Subscribe(_ =>
				{
					if (_timerHandler != null)
					{
						_disposables.Remove(_timerHandler);
						_timerHandler = null;
						_coinIcon.SetActive(false);
					}
				})
				.AddTo(_disposables);
		}

		private void OnDestroy()
		{
			_disposables.Dispose();
		}

		protected override void OnValidate()
		{
			Assert.IsNotNull(_coinIcon, "_coinIcon != null");
			Assert.IsNotNull(_counter, "_counter != null");

			base.OnValidate();
		}
	}
}