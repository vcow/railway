using GameScene.Models;
using Zenject;

namespace GameScene.Views
{
	public sealed class MineLabelView : LabelView
	{
		[Inject] private readonly IMineVertexModel _model;

		protected override void Start()
		{
			base.Start();
			LabelText = _model.Name;
		}
	}
}