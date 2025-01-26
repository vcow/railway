using EditorScene.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace EditorScene.Graph
{
	public class MineMarker : Marker
	{
		[SerializeField] private TextMeshProUGUI _nameLabel;

		protected override string GenerateDefaultName() => $"mine_{Id}";

		public float Multiplier { get; private set; }

		protected override void Start()
		{
			base.Start();

			Multiplier = _nodeModel?.Multiplier ?? 1f;

			_nameLabel.text = MarkerName;
			_signalBus.Subscribe<RenameMarkerSignal>(OnRenameMarker);
			_signalBus.Subscribe<ChangeMarkerMultiplierSignal>(OnChangeMultiplier);
		}

		protected override void OnDestroy()
		{
			_signalBus.Unsubscribe<RenameMarkerSignal>(OnRenameMarker);
			_signalBus.Unsubscribe<ChangeMarkerMultiplierSignal>(OnChangeMultiplier);

			base.OnDestroy();
		}

		protected override void OnValidate()
		{
			Assert.IsNotNull(_nameLabel, "_nameLabel != null");
			base.OnValidate();
		}

		private void OnRenameMarker(RenameMarkerSignal signal)
		{
			if (signal.Marker != this)
			{
				return;
			}

			MarkerName = signal.NewName;
			_nameLabel.text = signal.NewName;
		}

		private void OnChangeMultiplier(ChangeMarkerMultiplierSignal signal)
		{
			if (signal.Marker != this)
			{
				return;
			}

			Multiplier = signal.NewMultiplier;
		}
	}
}