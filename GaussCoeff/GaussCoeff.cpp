#define _USE_MATH_DEFINES
#include <iostream>
#include <string>
#include <cmath>
#include <sstream>
extern "C" __declspec(dllexport) double* GaussCoeff()
//int main()
{
	int r = 6; // -r 0 +r
	double sig = 3;
	double s = 0;
	double g;
	double* g_arr = new double[169];
	
	auto sig_sqr = 2.0 * sig * sig;
	auto pi_siq_sqr = sig_sqr * M_PI;
	int ind = 0;
	for (int i = -r; i <= r; ++i)
	{
		for (int j = -r; j <= r; ++j)
		{
			
			
			g = 1.0 / pi_siq_sqr * exp(-1.0 * (i * i + j * j) / (sig_sqr));
			s += g;
			g_arr[ind] = g;
			ind++;
		}

	}
	return g_arr;


}

extern "C" __declspec(dllexport) void FreeGaussCoeff(double* ptr)
{
	delete[] ptr; // Free the allocated memory
}