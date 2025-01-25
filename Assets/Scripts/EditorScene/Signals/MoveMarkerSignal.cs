using EditorScene.Graph;
using UnityEngine;

namespace EditorScene.Signals
{
	public class MoveMarkerSignal
	{
		public Marker Marker { get; }
		public Vector2 NewPosition { get; }

		public MoveMarkerSignal(Marker marker, Vector2 newPosition)
		{
			Marker = marker;
			NewPosition = newPosition;
		}
	}
}