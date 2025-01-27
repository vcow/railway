using GameScene.Models;
using UnityEngine;
using Zenject;

namespace GameScene.Views
{
	[DisallowMultipleComponent]
	public sealed class NodeView : MonoBehaviour
	{
		[Inject] private readonly INodeVertexModel _model;

		private void OnDrawGizmos()
		{
			Gizmos.DrawSphere(transform.position, 1f);
		}
	}
}