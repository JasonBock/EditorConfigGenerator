using System;
using System.Collections.Generic;
using System.Linq;

namespace EditorConfigGenerator.Core.Extensions
{
	internal static class ListOfUintsExtensions
	{
		internal static float GetConsistency(this List<uint> @this, uint total)
		{
			static IEnumerable<int> GetOtherValues(List<uint> values, uint max)
			{
				var found = false;
				foreach(var value in values)
				{
					if(value == max && !found)
					{
						found = true;
					}
					else
					{
						yield return (int)value;
					}
				}
			}

			if (@this is null) { throw new ArgumentNullException(nameof(@this)); }

			if(total == 0)
			{
				return 0f;
			}
			else if(@this.Count < 2)
			{
				return 0f;
			}
			else
			{
				var max = @this.Max();
				var average = GetOtherValues(@this, max).Average();
				return (max - (float)average) / total;
			}
		}
	}
}