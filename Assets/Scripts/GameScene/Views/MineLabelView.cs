using GameScene.Models;
using UniRx;
using Zenject;

namespace GameScene.Views
{
	public sealed class MineLabelView : LabelView
	{
		private readonly CompositeDisposable _disposables = new();

		[Inject] private readonly IMineVertexModel _model;

		protected override void Start()
		{
			base.Start();
			LabelText = _model.Name;

			_model.IsMining.Subscribe(b => EnableProgress = b).AddTo(_disposables);
			_model.Progress.Subscribe(f => ProgressValue = f).AddTo(_disposables);
		}

		private void OnDestroy()
		{
			_disposables.Dispose();
		}
	}
}