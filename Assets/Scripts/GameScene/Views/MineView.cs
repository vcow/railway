using GameScene.Models;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class MineView : MonoBehaviour
	{
		[SerializeField] private MineLabelView _labelPrefab;

		[Inject] private readonly DiContainer _container;
		[Inject] private readonly IMineVertexModel _model;
		[InjectOptional] private Canvas _labelCanvas;

		private void Start()
		{
			if (!_labelCanvas)
			{
				return;
			}

			_container.InstantiatePrefabForComponent<MineLabelView>(_labelPrefab, _labelCanvas.transform,
				new object[] { _model, transform });
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_labelPrefab, "_labelPrefab != null");
		}
	}
}