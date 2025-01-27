using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class TrainView : MonoBehaviour
	{
		[SerializeField] private TrainLabelView _labelPrefab;

		[Inject] private readonly DiContainer _container;
		[InjectOptional] private Canvas _labelCanvas;

		private void OnValidate()
		{
			Assert.IsNotNull(_labelPrefab, "_labelPrefab != null");
		}
	}
}