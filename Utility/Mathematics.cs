using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photozhop.Utility
{
    class Mathematics
    {
        public static float Clamp(float value, float min, float max)
		{
			if (value > max)
				return max;
			else if (value < min)
				return min;
			else
				return value;
		}

		public static byte Clamp(byte value, byte min, byte max)
		{
			if (value > max)
				return max;
			else if (value < min)
				return min;
			else
				return value;
		}
	}
}
