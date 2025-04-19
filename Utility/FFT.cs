using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace Photozhop.Utility
{
	class FFT
	{
		private static Complex[] ditfft(Complex[] arr, int x0, int N, int s)
		{
			Complex[] X = new Complex[N];
			if (N == 1)
				X[0] = arr[x0];
			else
			{
				ditfft(arr, x0, N / 2, 2 * s).CopyTo(X, 0);
				ditfft(arr, x0 + s, N / 2, 2 * s).CopyTo(X, N / 2);

				for (int k = 0; k < N / 2; ++k)
				{
					double u = 2 * Math.PI * k / N;
					var t = X[k];
					X[k] = t + new Complex(Math.Cos(u), Math.Sin(u)) * X[k + N / 2];
					X[k + N / 2] = t - new Complex(Math.Cos(u), Math.Sin(u)) * X[k + N / 2];


				}
			}
			return X;
		}

		public static Complex[] ditfft2d(Complex[] arr, int width, int height)
		{
			Complex[] X = new Complex[arr.Length];
			ParallelOptions opt = new ParallelOptions();
			if (Environment.ProcessorCount > 2)
				opt.MaxDegreeOfParallelism = Environment.ProcessorCount - 1;
			else
				opt.MaxDegreeOfParallelism = 1;

			Parallel.For(0, width, opt, (i) =>
			{
				Complex[] tmp = new Complex[width];
				Array.Copy(arr, i * width, tmp, 0, width);

				tmp = ditfft(tmp, 0, width, 1);
				for (int k = 0; k < width; k++)
				{
					X[i * width + k] = tmp[k] / width;
				}
			});

			Parallel.For(0, height, opt, (i) =>
			{
				Complex[] tmp = new Complex[height];
				for (int k = 0; k < height; k++)
					tmp[k] = X[i + k * width];

				tmp = ditfft(tmp, 0, tmp.Length, 1);
				for (int k = 0; k < width; k++)
					X[i + k * width] = tmp[k] / height;
			});
			return X;
		}
	}
}
