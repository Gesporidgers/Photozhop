using Photozhop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Photozhop.Models
{
	class MatrixTransformVM : BindHelper
	{
		private ImageModel src;
		private BitmapSource _image;
		private float[,] _array = new float[3, 3];
		private byte[] data;
		private ICommand applyFilter;

		public float[,] Array
		{
			get => _array;
			set
			{
				_array = value;
				OnPropertyChanged(nameof(Array));
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
					int size = src.Width * src.Height;
					data = new byte[src.Bytes.Length];
					src.Bytes.CopyTo(data, 0);
					for (int i = 0; i < size; i++)
					{
						int y = i / src.Width;
						int x = i - y * src.Width;
						int up = y - 3 / 2, down = y + 3 / 2;
						int left = x - 3 / 2, right = x + 3 / 2;
						float bSum = 0f, gSum = 0f, rSum = 0f;
						for (int a = up; a <= down; a++)
						{
							int Y = a;
							if (Y < 0) Y = Y + 3 - 1 - (a - up) * 2;
							else if (Y >= src.Height) Y = Y - 3 + 1 + (down - a) * 2;
							for (int b = left; b <= right; b++)
							{
								int X = b;
								if (X < 0) X = X + 3 - 1 - (b - left) * 2;
								else if (X >= src.Width) X = X - 3 + 1 + (right - b) * 2;
								bSum += data[Y * src.Width * 4 + X * 4] * _array[a - up, b - left];
								gSum += data[Y * src.Width * 4 + X * 4 + 1] * _array[a - up, b - left];
								rSum += data[Y * src.Width * 4 + X * 4 + 2] * _array[a - up, b - left];

							}

						}
						bSum = Mathematics.Clamp(bSum / 8, 0, 255);
						gSum = Mathematics.Clamp(gSum / 8, 0, 255);
						rSum = Mathematics.Clamp(rSum / 8, 0, 255);
						data[i * 4] = (byte)Math.Round(bSum, 0);
						data[i * 4 + 1] = (byte)Math.Round(gSum, 0);
						data[i * 4 + 2] = (byte)Math.Round(rSum, 0);
					}
					Image = BitmapSource.Create(src.Width, src.Height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null, data, src.Width * 4);
					Image.Freeze();
				});
			}
		}
	}
}
