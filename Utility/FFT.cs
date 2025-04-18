using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Photozhop.Utility
{
    class FFT
    {
        public static Complex[] ditfft(Complex[] arr,int x0,int N, int s)
        {
            Complex[] X = new Complex[N];
            if (N == 1)
                X[0] = arr[x0];
            else
            {
                ditfft(arr, x0, N / 2, 2 * s).CopyTo(X,0);
                ditfft(arr, x0 + s, N / 2, 2 * s).CopyTo(X, N/2);

			}
        }
    }
}
