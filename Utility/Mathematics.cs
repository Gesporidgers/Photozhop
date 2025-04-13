using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photozhop.Utility
{
	class Mathematics
	{
		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			if (value.CompareTo(max) > 0)
				return max;
			else if (value.CompareTo(min) < 0)
				return min;
			else
				return value;
		}
	}
}
