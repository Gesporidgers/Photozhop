using Photozhop.Utility;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photozhop.BinMethods
{
	internal class OtsuMethod : IBinaryzation
	{
		public string name => "Метод Отсу";
		public void Binaryze(byte[] data)
		{

			float[] N = new float[256];
			float[] sum_N = new float[256];
			float[] sum_iN = new float[256];
			uint maxI = 0;
			for (int i = 0; i < data.Length; i += 4)
			{
				N[data[i]] += 1 / data.Length;
				maxI = data[i] > maxI ? data[i] : maxI;
			}

			float sum = 0f, sum_i = 0f;
			for (int i = 0; i < maxI; i++)
			{
				sum += N[i];
				sum_i += i * N[i];
				sum_N[i] = sum;
				sum_iN[i] = sum_i;
			}

			float omega1 = 0f, omega2 = 0f, mu1 = 0f, mu2 = 0f;
			float sigma = 0f; int threshold = 0;

			for (int t = 1; t <= maxI; t++)
			{
				omega1 = N[t-1];
				omega2 = 1f - omega1;
				mu1 = sum_iN[t - 1] / omega1;
				mu2 = (sum_iN[maxI] - mu1 * omega1) / omega2;
				float sig = omega1 * omega2 * (mu1 - mu2) * (mu1 - mu2);
				if (sig > sigma)
				{
					sigma = sig;
					threshold = t;
				}
			}

			for (int i = 0; i < data.Length; i+=4)
			{
				if(data[i] > threshold)
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
