using Photozhop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Photozhop.Models
{
	class MatrixTransformVM : BindHelper
	{
		private ImageModel src;
		private BitmapSource _image;
		private float[,] _array = new float[3, 3];
		private byte[] data;
		private ICommand applyFilter;
		private string _filter;
		private bool _matEnabled;
		private ICommand updateSize;
		private int radius = 3;

		public string[] Filters => new string[] { "Matrix Transform", "Median Blur" };
		public float[,] Array
		{
			get => _array;
			set
			{
				_array = value;
				OnPropertyChanged(nameof(Array));
			}
		}

		public bool MatEnabled
		{
			get => _matEnabled;
			set
			{
				_matEnabled = value;
				OnPropertyChanged(nameof(MatEnabled));
			}
		}

		public int Radius
		{
			get => radius;
			set
			{
				radius = value;
				OnPropertyChanged(nameof(Radius));
			}
		}

		public string Filter
		{
			get => _filter;
			set
			{
				if (value == "Matrix Transform")
					MatEnabled = true;
				else
					MatEnabled = false;
				_filter = value;
				OnPropertyChanged(nameof(Filter));
			}
		}

		public BitmapSource Image
		{
			get => _image;
			set
			{
				_image = value;
				OnPropertyChanged(nameof(Image));
			}
		}

		public MatrixTransformVM(ref ImageModel imageModel)
		{
			this.src = imageModel;
			Image = src.Bitmap;
		}

		public ICommand ApplyFilter
		{
			get
			{
				return applyFilter ??= new RelayCommand((_) => true, (_) =>
				{
					switch (Filter)
					{
						case "Matrix Transform":
							MatrixTransform();
							break;
						case "Median Blur":
							MedianBlur();
							break;
						default:
							break;
					}

				});
			}
		}

		public ICommand AdjustMatrixDimensions
		{
			get
			{
				return updateSize ??= new RelayCommand((_) => MatEnabled, (_) =>
				{
					if (Filter == "Matrix Transform")
						Array = new float[Radius, Radius];
				});
			}
		}

		public void MatrixTransform()
		{
			int size = src.Width * src.Height;
			//data = new byte[src.Bytes.Length];
			byte[] data_copy = new byte[src.Bytes.Length];
			src.Bytes.CopyTo(data_copy, 0);
			//src.Bytes.CopyTo(data, 0);
			Parallel.For(0, size, (i) =>
			//for (int i = 0; i < size; i++)
			{
				int y = i / src.Width;
				int x = i - y * src.Width;
				int up = y - _array.GetLength(0) / 2, down = y + _array.GetLength(0) / 2;
				int left = x - _array.GetLength(1) / 2, right = x + _array.GetLength(1) / 2;
				float bSum = 0f, gSum = 0f, rSum = 0f;
				for (int a = up; a <= down; a++)
				{
					int Y = a;
					if (Y < 0) Y = Y + _array.GetLength(0) - 1 - (a - up) * 2;
					else if (Y >= src.Height) Y = Y - _array.GetLength(0) + 1 + (down - a) * 2;
					for (int b = left; b <= right; b++)
					{
						int X = b;
						if (X < 0) X = X + _array.GetLength(1) - 1 - (b - left) * 2;
						else if (X >= src.Width) X = X - _array.GetLength(0) + 1 + (right - b) * 2;
						bSum += src.Bytes[Y * src.Width * 4 + X * 4] * _array[a - up, b - left];
						gSum += src.Bytes[Y * src.Width * 4 + X * 4 + 1] * _array[a - up, b - left];
						rSum += src.Bytes[Y * src.Width * 4 + X * 4 + 2] * _array[a - up, b - left];

					}

				}
				bSum = Mathematics.Clamp(bSum, 0, 255);
				gSum = Mathematics.Clamp(gSum, 0, 255);
				rSum = Mathematics.Clamp(rSum, 0, 255);
				data_copy[i * 4] = (byte)Math.Round(bSum, 0);
				data_copy[i * 4 + 1] = (byte)Math.Round(gSum, 0);
				data_copy[i * 4 + 2] = (byte)Math.Round(rSum, 0);

			}); Image = BitmapSource.Create(src.Width, src.Height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null, data_copy, src.Width * 4);
			Image.Freeze();
		}

		public void MedianBlur()
		{
			int size = src.Width * src.Height;
			byte[] data_copy = new byte[src.Bytes.Length];
			src.Bytes.CopyTo(data_copy, 0);
			Parallel.For(0, size, (i) =>
			//for (int i = 0; i < size; i++)
			{
				(int, int) x_rad = (Radius / 2, Radius / 2);
				(int, int) y_rad = x_rad;
				int y = i / src.Width;
				int x = i - y * src.Width;
				int _i = y * src.Width + x;

				if (x - x_rad.Item1 < 0) x_rad.Item1 = x;
				if (x + x_rad.Item2 >= src.Width) x_rad.Item2 = src.Width - x - 1;
				if (y - y_rad.Item1 < 0) y_rad.Item1 = y;
				if (y + y_rad.Item2 >= src.Height) y_rad.Item2 = src.Height - y - 1;
				uint len = (uint)((x_rad.Item1 + x_rad.Item2 + 1) * (y_rad.Item1 + y_rad.Item2 + 1));
				byte[] rangeB = new byte[len];
				byte[] rangeG = new byte[len];
				byte[] rangeR = new byte[len];
				int ii = 0;
				for (int _x = x - x_rad.Item1; _x <= x + x_rad.Item2; _x++)
				{
					for (int _y = y - y_rad.Item1; _y <= y + y_rad.Item2; _y++)
					{
						_i = _y * src.Width + _x;
						rangeB[ii] = src.Bytes[_i * 4];
						rangeG[ii] = src.Bytes[_i * 4 + 1];
						rangeR[ii] = src.Bytes[_i * 4 + 2];
						ii++;
					}
				}
				MathNet.Numerics.Sorting.Sort(rangeB);
				MathNet.Numerics.Sorting.Sort(rangeG);
				MathNet.Numerics.Sorting.Sort(rangeR);
				data_copy[i * 4] = rangeB[rangeB.Length / 2];
				data_copy[i * 4 + 1] = rangeG[rangeG.Length / 2];
				data_copy[i * 4 + 2] = rangeR[rangeR.Length / 2];
			});
			Image = BitmapSource.Create(src.Width, src.Height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null, data_copy, src.Width * 4);
			Image.Freeze();
		}
	}
}
