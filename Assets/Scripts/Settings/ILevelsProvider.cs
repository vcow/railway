using System.Collections.Generic;
using Models;

namespace Settings
{
	public interface ILevelsProvider
	{
		IReadOnlyList<ILevelModel> Levels { get; }
	}
}