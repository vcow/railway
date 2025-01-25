namespace EditorScene.Graph
{
	public class NodeMarker : Marker
	{
		protected override string GenerateDefaultName() => $"node_{Id}";
	}
}