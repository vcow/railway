namespace EditorScene.Signals
{
	public sealed class SetLevelNameSignal
	{
		public string Name { get; }

		public SetLevelNameSignal(string name)
		{
			Name = name;
		}
	}
}