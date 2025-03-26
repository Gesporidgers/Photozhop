using Photozhop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photozhop.BinMethods
{
	internal class GavrMethod : IBinaryzation
	{
		public string name => "Метод Гаврилова";
		public void Binaryze(ref byte[] data)
		{
			uint intensity = 0;
			uint threshold;
			for (int i = 0; i < data.Length; i += 4)
				intensity = data[i];
			threshold = (uint)(intensity / data.Length);
			for (int i = 0; i < data.Length; i += 4)
			{
				if (data[i] > threshold)
				{
					data[i] = 255;
					data[i + 1] = 255;
					data[i + 2] = 255;
				}
				else
				{
					data[i] = 0;
					data[i + 1] = 0;
					data[i + 2] = 0;
				}
			}
		}
	}
}
