using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
	public abstract class LabelView : MonoBehaviour
	{
		[SerializeField] private Vector3 _labelOffset;
		[SerializeField] private TextMeshProUGUI _label;
		[SerializeField] private GameObject _progress;
		[SerializeField] private Image _bar;

		[Inject] private readonly Transform _targetTransform;

		private bool? _enableProgress;
		private float _progressValue;

		private Camera _camera;
		private RectTransform _canvasTransform;
		private RectTransform _transform;

		protected bool EnableProgress
		{
			get => _enableProgress ?? false;
			set
			{
				if (value == _enableProgress)
				{
					return;
				}

				_enableProgress = value;
				_progress.gameObject.SetActive(value);
			}
		}

		protected float ProgressValue
		{
			get => _progressValue;
			set
			{
				value = Mathf.Clamp01(value);
				if (value.Equals(_progressValue))
				{
					return;
				}

				_progressValue = value;
				_bar.fillAmount = value;
			}
		}

		protected string LabelText
		{
			get => _label.text;
			set => _label.text = value;
		}

		protected virtual void Awake()
		{
			var canvas = GetComponentInParent<Canvas>();
			Assert.IsNotNull(canvas);
			_canvasTransform = (RectTransform)canvas.transform;
			_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			Assert.IsNotNull(_camera);
			_transform = (RectTransform)transform;
		}

		protected virtual void Start()
		{
			if (!_enableProgress.HasValue)
			{
				EnableProgress = false;
			}
		}

		private void Update()
		{
			var position = _targetTransform.position + _labelOffset;
			var viewportPoint = _camera.WorldToViewportPoint(position);
			var sizeDelta = _canvasTransform.sizeDelta;
			_transform.anchoredPosition = new Vector2(viewportPoint.x * sizeDelta.x - sizeDelta.x * 0.5f,
				viewportPoint.y * sizeDelta.y - sizeDelta.y * 0.5f);
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_label, "_label != null");
			Assert.IsNotNull(_progress, "_progress != null");
			Assert.IsNotNull(_bar, "_bar != null");
		}
	}
}