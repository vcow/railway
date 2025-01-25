using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace Settings
{
	[CreateAssetMenu(fileName = "LevelsProvider", menuName = "Game/Levels Provider")]
	public sealed class LevelsProvider : ScriptableObjectInstaller<LevelsProvider>, ILevelsProvider
	{
		[SerializeField] private TextAsset[] _levels;

		private IReadOnlyList<ILevelModel> _levelModels;

		public IReadOnlyList<ILevelModel> Levels
		{
			get
			{
				if (_levelModels == null)
				{
					throw new Exception("LevelsProvider must be initialized first.");
				}

				return _levelModels;
			}
		}

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<LevelsProvider>().FromInstance(this).AsSingle();
		}

		public UniTask Initialize()
		{
			if (_levelModels != null)
			{
				throw new Exception("LevelsProvider is already initialized.");
			}

			_levelModels = _levels.Select(asset => DeserializeLevel(asset.text)).ToArray();
			return UniTask.CompletedTask;
		}

		private ILevelModel DeserializeLevel(string rawData)
		{
			var level = JsonConvert.DeserializeObject<LevelModelImpl>(rawData);
			return level;
		}
	}
}