using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Models
{
	internal class LevelModelImpl : ILevelModel
	{
		[JsonProperty(PropertyName = "name")] public string Name { get; set; }
		[JsonProperty(PropertyName = "nodes")] public List<NodeRecord> Nodes { get; set; } = new();
		[JsonProperty(PropertyName = "connections")] public List<ConnectionRecord> Connections { get; set; } = new();
		[JsonProperty(PropertyName = "trains")] public List<TrainModelImpl> Trains { get; set; } = new();

		[JsonIgnore] IEnumerable<INodeModel> ILevelModel.Nodes => Nodes;
		[JsonIgnore] IEnumerable<IConnectionModel> ILevelModel.Connections => Connections;
		[JsonIgnore] IEnumerable<ITrainModel> ILevelModel.Trains => Trains;

		public LevelModelImpl()
		{
		}

		public LevelModelImpl(ILevelModel initialLevelModel)
		{
			Name = initialLevelModel.Name;
			Nodes.AddRange(initialLevelModel.Nodes.Select(model => new NodeRecord
			{
				Id = model.Id,
				Multiplier = model.Multiplier,
				Name = model.Name,
				Type = model.Type,
				XPos = model.XPos,
				YPos = model.YPos
			}));
			Connections.AddRange(initialLevelModel.Connections.Select(model => new ConnectionRecord
			{
				Length = model.Length,
				FromNodeId = model.FromNodeId,
				ToNodeId = model.ToNodeId
			}));
			Trains.AddRange(initialLevelModel.Trains.Select(model => new TrainModelImpl
			{
				Name = model.Name,
				Speed = model.Speed,
				Mining = model.Mining
			}));
		}

		public class NodeRecord : INodeModel
		{
			[JsonProperty(PropertyName = "type"), JsonConverter(typeof(StringEnumConverter))] public NodeType Type { get; set; }
			[JsonProperty(PropertyName = "id")] public int Id { get; set; }
			[JsonProperty(PropertyName = "name")] public string Name { get; set; }
			[JsonProperty(PropertyName = "x")] public float XPos { get; set; }
			[JsonProperty(PropertyName = "y")] public float YPos { get; set; }
			[JsonProperty(PropertyName = "multiplier")] public float Multiplier { get; set; }
		}

		public class ConnectionRecord : IConnectionModel
		{
			[JsonProperty(PropertyName = "length")] public float Length { get; set; }
			[JsonProperty(PropertyName = "from")] public int FromNodeId { get; set; }
			[JsonProperty(PropertyName = "to")] public int ToNodeId { get; set; }
		}
	}
}