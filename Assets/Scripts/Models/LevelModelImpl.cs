using Newtonsoft.Json;

namespace Models
{
	internal class LevelModelImpl : ILevelModel
	{
		[JsonProperty(PropertyName = "name")] public string Name { get; set; }
	}
}