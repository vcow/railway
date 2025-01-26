using System.Collections.Generic;

namespace Models
{
	public enum NodeType
	{
		Node,
		Mine,
		Base
	}

	public interface INodeModel
	{
		NodeType Type { get; }
		int Id { get; }
		string Name { get; }
		float XPos { get; }
		float YPos { get; }
		float Multiplier { get; }
	}

	public interface IConnectionModel
	{
		int FromNodeId { get; }
		int ToNodeId { get; }
	}

	public interface ITrainModel
	{
		string Name { get; }
		float Speed { get; }
		float Mining { get; }
	}

	public interface ILevelModel
	{
		string Name { get; }
		IEnumerable<INodeModel> Nodes { get; }
		IEnumerable<IConnectionModel> Connections { get; }
		IEnumerable<ITrainModel> Trains { get; }
	}
}