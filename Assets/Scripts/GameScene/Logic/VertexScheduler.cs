using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameScene.Logic
{
	public sealed class VertexScheduler
	{
		public const float TimeGap = 1f;

		private readonly SortedSet<Gap> _gaps = new();

		public float ScheduleArrival(float estimatedArrivalTime)
		{
			var now = Time.time;
			Assert.IsTrue(estimatedArrivalTime >= now);

			_gaps.RemoveWhere(gap => gap.To < now);

			var newGap = new Gap(estimatedArrivalTime, estimatedArrivalTime + TimeGap);
			foreach (var gap in _gaps)
			{
				if (!Intersects(gap, newGap))
				{
					continue;
				}

				newGap = new Gap(gap.To, gap.To + TimeGap);
			}

			_gaps.Add(newGap);

			return newGap.From;
		}

		private bool Intersects(Gap gap1, Gap gap2) =>
			gap2.From >= gap1.From && gap2.From < gap1.To || gap2.To >= gap1.From && gap2.To < gap1.To;

		private readonly struct Gap : IComparable<Gap>
		{
			public readonly float From;
			public readonly float To;

			public int CompareTo(Gap other)
			{
				if (From > other.From) return 1;
				if (From < other.From) return -1;
				return 0;
			}

			public Gap(float from, float to)
			{
				From = from;
				To = to;
			}
		}
	}
}