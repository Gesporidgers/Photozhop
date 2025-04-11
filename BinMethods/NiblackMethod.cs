using Photozhop.Utility;
using MathNet.Numerics.Statistics;
using System;
using System.Threading.Tasks;

namespace Photozhop.BinMethods
{
	class NiblackMethod : IBinaryzation
	{
		public string name => "Метод Ниблекса";
		private int radius;
		private float k;
		private byte[] data;
		private int width;
		private int height;
		public void Binaryze(ref byte[] data)
		{
			byte[] copy =new byte[data.Length]; data.CopyTo(copy, 0);
			byte[] data_copy = new byte[data.Length]; data.CopyTo(data_copy, 0);
			copy[0] = (byte)(255 - data[0]);
			int size = width * height;
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
						ii++;
					}
				}
				MX = range.Mean();
				si = range.StandardDeviation();
				double t = MX + k * si;
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

		public void SetParams(int rad, float k, int w, int h)
		{
			this.radius = rad;
			this.k = k;
			this.width = w;
			this.height = h;
		}
	}
}
