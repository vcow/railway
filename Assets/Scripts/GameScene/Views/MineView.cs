using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class MineView : MonoBehaviour
	{
		[SerializeField] private TextMeshPro _label;

		private void OnValidate()
		{
			Assert.IsNotNull(_label, "_label != null");
		}
	}
}