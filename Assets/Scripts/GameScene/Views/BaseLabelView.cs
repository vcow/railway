using GameScene.Models;
using Zenject;

namespace GameScene.Views
{
	public sealed class BaseLabelView : LabelView
	{
		[Inject] private readonly IBaseVertexModel _model;

		protected override void Start()
		{
			base.Start();
			LabelText = _model.Name;
		}
	}
}