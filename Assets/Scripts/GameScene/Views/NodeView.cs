using GameScene.Models;
using UnityEditor;
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
			Handles.Label(transform.position, _model.Id.ToString());
		}
	}
}