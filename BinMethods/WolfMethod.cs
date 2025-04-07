using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using Photozhop.Utility;

namespace Photozhop.BinMethods
{
    class WolfMethod : IBinaryzation
    {
		public string name => "Метод Кристиана Вульфа";
		private int radius;
		private byte[] data;
		private int width;
		private int height;
		public void Binaryze(ref byte[] data)
		{
			byte[] copy = new byte[data.Length]; data.CopyTo(copy, 0);
			byte[] data_copy = new byte[data.Length]; data.CopyTo(data_copy, 0);
			copy[0] = (byte)(255 - data[0]);
			int size = width * height; byte minI = 255;
			double maxSig = 0.0;
			//for (int i = 0; i < size; i++)
			Parallel.For(0, size, (i) =>
			{
				(int, int) x_rad = (radius, radius);
				(int, int) y_rad = x_rad;
				int y = i / width;
				int x = i - y * width;
				int _i = y * width + x;

				if (x - x_rad.Item1 < 0) x_rad.Item1 = x;
				if (x + x_rad.Item2 >= width) x_rad.Item2 = width - x - 1;
				if (y - y_rad.Item1 < 0) y_rad.Item1 = y;
				if (y + y_rad.Item2 >= height) y_rad.Item2 = height - y - 1;

				uint len = (uint)((x_rad.Item1 + x_rad.Item2 + 1) * (y_rad.Item1 + y_rad.Item2 + 1));
				double[] range = new double[len];
				int ii = 0;
				double MX, DX, si;
				for (int _x = x - x_rad.Item1; _x <= x + x_rad.Item2; _x++)
				{
					for (int _y = y - y_rad.Item1; _y <= y + y_rad.Item2; _y++)
					{
						_i = _y * width + _x;
						range[ii] = copy[_i * 4];
						minI = Math.Min(minI, copy[_i * 4]);
						ii++;
					}
				}
				MX = range.Mean();
				si = range.StandardDeviation();
				maxSig = Math.Max(si, maxSig);
				double t = 0.5 * MX + 0.5 * minI + 0.5 * (si / maxSig) * (MX - minI);
				if (copy[i * 4] <= t)
				{
					data_copy[i * 4] = 0;
					data_copy[i * 4 + 1] = 0;
					data_copy[i * 4 + 2] = 0;
				}
				else
				{
					data_copy[i * 4] = 255;
					data_copy[i * 4 + 1] = 255;
					data_copy[i * 4 + 2] = 255;
				}
			});
			data_copy.CopyTo(data, 0);
		}

		public void SetParams(int rad, int w, int h)
		{
			this.radius = rad;
			this.width = w;
			this.height = h;
		}
	}
}
