using System;
using GameScene.Models;
using Models;
using UniRx;

namespace GameScene.Controllers
{
	public class GameModelController : IDisposable
	{
		private readonly CompositeDisposable _disposables;

		private readonly ILevelModel _levelModel;
		private readonly GameModelImpl _gameModel;

		public IGameModel GameModel => _gameModel;

		public GameModelController(ILevelModel levelModel)
		{
			_levelModel = levelModel;
			_gameModel = new GameModelImpl();
			_disposables = new CompositeDisposable(_gameModel);
		}

		void IDisposable.Dispose()
		{
			_disposables.Dispose();
		}
	}
}