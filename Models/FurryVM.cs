using Photozhop.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Photozhop.Models
{
	internal class FurryVM : BindHelper
	{
		private ImageModel src;
		private BitmapSource _image;
		private BitmapSource _furryImage;
		private ICommand makeFurry;

		public BitmapSource Image
		{
			get => _image;
			set
			{
				_image = value;
				OnPropertyChanged(nameof(Image));
			}
		}

		public BitmapSource FurryImage
		{
			get => _furryImage;
			set
			{
				_furryImage = value;
				OnPropertyChanged(nameof(FurryImage));
			}
		}

		public FurryVM(ref ImageModel src)
		{
			this.src = src;
			Image = src.Bitmap;
		}

		public ICommand MakeFurry => makeFurry ??= new RelayCommand((_) => true, (_) =>
		{
			FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
			convertedBitmap.BeginInit();
			convertedBitmap.Source = Image;
			convertedBitmap.DestinationFormat = System.Windows.Media.PixelFormats.Bgr32;
			convertedBitmap.EndInit();

			int stride = (int)convertedBitmap.PixelWidth * (convertedBitmap.Format.BitsPerPixel / 8);
			byte[] b = new byte[convertedBitmap.PixelHeight * stride];
			convertedBitmap.CopyPixels(b, stride, 0);
			int width = src.Width;
			int height = src.Height;

			int new_width = width;
			int new_height = height;
			var p = Math.Log(width, 2);
			if (p != Math.Floor(p))
				new_width = (int)Math.Pow(2, Math.Ceiling(p));
			p = Math.Log(height, 2);
			if (p != Math.Floor(p))
				new_height = (int)Math.Pow(2, Math.Ceiling(p));

			byte[] furry_bytes = new byte[new_width * new_height*4];
			byte[] bytes = new byte[new_width * new_height*4];
			b.CopyTo(bytes, 0);

			Complex[] complexes = new Complex[new_width * new_height];
			for (int color = 0; color < 2; color++)
			{
				for (int i  = 0; i < new_width * new_height; i++)
				{
					int y = i / new_width;
					int x = i % new_width;
					complexes[i] = Math.Pow(-1,x+y) * bytes[i*3+color];
				}

				complexes = FFT.ditfft2d(complexes,new_width,new_height);
				var max_ma = complexes.Max(x => Mathematics.F(x.Imaginary));

				for (int i = 0;i < new_width * new_height; i++)
				{
					int y = i / new_width;
					int x = i % new_width;
					y -= new_height / 2;
					x -= new_width / 2;

					furry_bytes[i * 4 + color] = (byte)Mathematics.Clamp(10.0 * Mathematics.F(complexes[i].Magnitude) * 255 / max_ma, 0, 255);
				}
			}

			FurryImage = BitmapSource.Create(new_width, new_height,96,96, System.Windows.Media.PixelFormats.Bgr32,null,furry_bytes,new_width*4);
		});
	}
}
