using System;
using System.Linq;
using GameScene.Logic;
using GameScene.Models;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene
{
	[DisallowMultipleComponent]
	public sealed class GameSceneController : MonoBehaviour
	{
		private readonly CompositeDisposable _disposables = new();

		[SerializeField] private TextMeshProUGUI _scoresCounter;

		[Inject] private readonly ZenjectSceneLoader _sceneLoader;
		[Inject] private readonly GameLogic _gameLogic;
		[Inject] private readonly IGameModel _gameModel;

		private void Start()
		{
			_gameModel.Bases.Select(model => model.Resources).CombineLatest()
				.Subscribe(list => _scoresCounter.text = list.Sum().ToString("F2")).AddTo(_disposables);
		}

		public void OnExit()
		{
			_sceneLoader.LoadSceneAsync(Const.StartSceneName);
		}

		private void OnDestroy()
		{
			_disposables.Dispose();
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_scoresCounter, "_scoresCounter != null");
		}
	}
}