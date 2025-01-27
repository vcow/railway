using GameScene.Models;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class BaseView : MonoBehaviour
	{
		[SerializeField] private BaseLabelView _labelPrefab;

		[Inject] private readonly DiContainer _container;
		[Inject] private readonly IBaseVertexModel _model;
		[InjectOptional] private Canvas _labelCanvas;

		private void Start()
		{
			if (!_labelCanvas)
			{
				return;
			}

			_container.InstantiatePrefabForComponent<BaseLabelView>(_labelPrefab, _labelCanvas.transform,
				new object[] { _model, transform });
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_labelPrefab, "_labelPrefab != null");
		}
	}
}