using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace EditorScene
{
	[DisallowMultipleComponent]
	public class Grid : MonoBehaviour
	{
		[Header("Grid"), SerializeField] private Vector2Int _size;
		[Header("Line"), SerializeField] private Sprite _pen;
		[SerializeField, Range(1f, 10f)] private float _thickness = 2f;
		[SerializeField] private Color _color = Color.white;

		private void Start()
		{
			if (_size.x <= 0 || _size.y <= 0)
			{
				return;
			}

			var rt = (RectTransform)transform;
			var gridSize = rt.rect.size;
			var stepX = gridSize.x / _size.x;
			var stepY = gridSize.y / _size.y;

			for (var x = 0; x < _size.x + 1; ++x)
			{
				var line = new GameObject($"vertical_{x + 1}", typeof(RectTransform), typeof(Image));

				var lineTransform = (RectTransform)line.transform;
				lineTransform.SetParent(rt);
				lineTransform.pivot = new Vector2(0.5f, 0f);
				lineTransform.anchorMin = Vector2.zero;
				lineTransform.anchorMax = Vector2.zero;
				lineTransform.sizeDelta = new Vector2(_thickness, gridSize.y);
				lineTransform.anchoredPosition = new Vector2(x * stepX, 0f);

				var lineImage = line.GetComponent<Image>();
				lineImage.sprite = _pen;
				lineImage.color = _color;
				lineImage.raycastTarget = false;
			}

			for (var y = 0; y < _size.y + 1; ++y)
			{
				var line = new GameObject($"horizontal_{y + 1}", typeof(RectTransform), typeof(Image));

				var lineTransform = (RectTransform)line.transform;
				lineTransform.SetParent(rt);
				lineTransform.pivot = new Vector2(0f, 0.5f);
				lineTransform.anchorMin = Vector2.zero;
				lineTransform.anchorMax = Vector2.zero;
				lineTransform.sizeDelta = new Vector2( gridSize.x, _thickness);
				lineTransform.anchoredPosition = new Vector2(0f, y * stepY);

				var lineImage = line.GetComponent<Image>();
				lineImage.sprite = _pen;
				lineImage.color = _color;
				lineImage.raycastTarget = false;
			}
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_pen, "_pen != null");
		}
	}
}