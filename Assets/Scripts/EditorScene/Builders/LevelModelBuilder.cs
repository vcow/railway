using System;
using System.Linq;
using EditorScene.Graph;
using EditorScene.Signals;
using Models;
using Newtonsoft.Json;
using Settings;
using Zenject;

namespace EditorScene.Builders
{
	public class LevelModelBuilder : IDisposable
	{
		private readonly SignalBus _signalBus;
		private readonly LevelModelImpl _levelModel;

		public ILevelModel LevelModel => _levelModel;

		public LevelModelBuilder(LevelsProvider levelsProvider, SignalBus signalBus, [InjectOptional] ILevelModel initialLevelModel)
		{
			_signalBus = signalBus;

			if (initialLevelModel == null)
			{
				var defaultName = $"level {levelsProvider.Levels.Count + 1}";
				_levelModel = new LevelModelImpl
				{
					Name = defaultName
				};
			}
			else
			{
				_levelModel = new LevelModelImpl(initialLevelModel);
			}

			_signalBus.Subscribe<TrainsListChangedSignal>(OnChangeTrainsList);
			_signalBus.Subscribe<SelectMarkerSignal>(OnSelectMarker);
			_signalBus.Subscribe<SetLevelNameSignal>(OnSetLevelName);
			_signalBus.Subscribe<RenameMarkerSignal>(OnRenameMarker);
			_signalBus.Subscribe<RemoveMarkerSignal>(OnRemoveMarkerSignal);
			_signalBus.Subscribe<SetConnectionLengthSignal>(OnSetConnectionLength);
			_signalBus.Subscribe<RemoveConnectionSignal>(OnRemoveConnection);
			_signalBus.Subscribe<MoveMarkerSignal>(OnMoveMarker);
			_signalBus.Subscribe<DragMarkerSignal>(OnDragMarker);
			_signalBus.Subscribe<ChangeMarkerMultiplierSignal>(OnChangeMarkerMultiplier);
		}

		void IDisposable.Dispose()
		{
			_signalBus.Unsubscribe<TrainsListChangedSignal>(OnChangeTrainsList);
			_signalBus.Unsubscribe<SelectMarkerSignal>(OnSelectMarker);
			_signalBus.Unsubscribe<SetLevelNameSignal>(OnSetLevelName);
			_signalBus.Unsubscribe<RenameMarkerSignal>(OnRenameMarker);
			_signalBus.Unsubscribe<RemoveMarkerSignal>(OnRemoveMarkerSignal);
			_signalBus.Unsubscribe<SetConnectionLengthSignal>(OnSetConnectionLength);
			_signalBus.Unsubscribe<RemoveConnectionSignal>(OnRemoveConnection);
			_signalBus.Unsubscribe<MoveMarkerSignal>(OnMoveMarker);
			_signalBus.Unsubscribe<DragMarkerSignal>(OnDragMarker);
			_signalBus.Unsubscribe<ChangeMarkerMultiplierSignal>(OnChangeMarkerMultiplier);
		}

		private void OnChangeTrainsList(TrainsListChangedSignal signal)
		{
			_levelModel.Trains.Clear();
			_levelModel.Trains.AddRange(signal.Trains);
		}

		private void OnSelectMarker(SelectMarkerSignal signal)
		{
			if (signal.Marker != null)
			{
				GetOrCreateNodeRecord(signal.Marker);
			}
		}

		private void OnSetLevelName(SetLevelNameSignal signal)
		{
			_levelModel.Name = signal.Name;
		}

		private void OnRenameMarker(RenameMarkerSignal signal)
		{
			var record = GetOrCreateNodeRecord(signal.Marker);
			record.Name = signal.NewName;
		}

		private void OnRemoveMarkerSignal(RemoveMarkerSignal signal)
		{
			var record = GetOrCreateNodeRecord(signal.Marker);
			_levelModel.Nodes.Remove(record);
		}

		private void OnSetConnectionLength(SetConnectionLengthSignal signal)
		{
			var record = GetOrCreateConnectionRecord(signal.NodeConnection);
			record.Length = signal.Length;
		}

		private void OnRemoveConnection(RemoveConnectionSignal signal)
		{
			var record = GetOrCreateConnectionRecord(signal.NodeConnection);
			_levelModel.Connections.Remove(record);
		}

		private void OnMoveMarker(MoveMarkerSignal signal)
		{
			var record = GetOrCreateNodeRecord(signal.Marker);
			record.XPos = signal.Marker.Position.x;
			record.YPos = signal.Marker.Position.y;
		}

		private void OnDragMarker(DragMarkerSignal signal)
		{
			var record = GetOrCreateNodeRecord(signal.Marker);
			record.XPos = signal.Marker.Position.x;
			record.YPos = signal.Marker.Position.y;
		}

		private void OnChangeMarkerMultiplier(ChangeMarkerMultiplierSignal signal)
		{
			var record = GetOrCreateNodeRecord(signal.Marker);
			record.Multiplier = signal.NewMultiplier;
		}

		private LevelModelImpl.NodeRecord GetOrCreateNodeRecord(Marker marker)
		{
			var record = _levelModel.Nodes.FirstOrDefault(nodeRecord => nodeRecord.Id == marker.Id);
			if (record == null)
			{
				record = new LevelModelImpl.NodeRecord
				{
					Id = marker.Id,
					Name = marker.MarkerName,
					Type = marker switch
					{
						NodeMarker => NodeType.Node,
						MineMarker => NodeType.Mine,
						BaseMarker => NodeType.Base,
						_ => throw new NotSupportedException()
					},
					Multiplier = marker switch
					{
						MineMarker mineMarker => mineMarker.Multiplier,
						BaseMarker baseMarker => baseMarker.Multiplier,
						_ => 1f
					},
					XPos = marker.Position.x,
					YPos = marker.Position.y
				};
				_levelModel.Nodes.Add(record);
			}

			return record;
		}

		private LevelModelImpl.ConnectionRecord GetOrCreateConnectionRecord(NodeConnection connection)
		{
			var record = _levelModel.Connections.FirstOrDefault(
				nodeRecord => nodeRecord.FromNodeId == connection.From.Id &&
				              nodeRecord.ToNodeId == connection.To.Id);
			if (record == null)
			{
				record = new LevelModelImpl.ConnectionRecord
				{
					Length = connection.Length,
					FromNodeId = connection.From.Id,
					ToNodeId = connection.To.Id
				};
				_levelModel.Connections.Add(record);
			}

			return record;
		}

		public string GetSerializedData()
		{
			var data = JsonConvert.SerializeObject(_levelModel);
			return data;
		}
	}
}