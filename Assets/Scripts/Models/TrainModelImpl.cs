using Newtonsoft.Json;

namespace Models
{
	public class TrainModelImpl : ITrainModel
	{
		[JsonProperty(PropertyName = "name")] public string Name { get; set; }
		[JsonProperty(PropertyName = "speed")] public float Speed { get; set; }
		[JsonProperty(PropertyName = "mining")] public float Mining { get; set; }
	}
}