using GameScene.Models;
using Zenject;

namespace GameScene.Views
{
	public sealed class TrainLabelView : LabelView
	{
		[Inject] private readonly ITrainObjectModel _model;

		protected override void Start()
		{
			base.Start();
			LabelText = _model.Name;
		}
	}
}