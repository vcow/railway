using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class ConnectionView : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _view;
		[SerializeField] private float _width;

		[Inject] private readonly Quaternion _rotation;
		[Inject] private readonly float _length;

		private void Start()
		{
			_view.transform.rotation = _rotation;
			_view.size = new Vector2(_length, _width);
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_view, "_view != null");
		}
	}
}