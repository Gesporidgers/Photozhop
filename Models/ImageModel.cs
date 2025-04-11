using Photozhop.Utility;
using System.Windows.Media.Imaging;

namespace Photozhop.Models
{
	public class ImageModel : BindHelper
	{
		private string _name;
		private double _opacity = 1;
		private PPO _selectedOperation = PPO.getPPO()[0];
		private BitmapSource _bitmap;
		public int Width { get; set; }
		public int Height { get; set; }
		public double Opacity
		{
			get => _opacity * 100;
			set
			{
				_opacity = value / 100;
				OnPropertyChanged(nameof(Opacity));
			}
		}
		public byte[] Bytes { private set; get; }

		public PPO SelectedOperation
		{
			get => _selectedOperation;
			set
			{
				_selectedOperation = value;
				OnPropertyChanged(nameof(SelectedOperation));
			}
		}
		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public BitmapSource Bitmap
		{
			get => _bitmap;
			set
			{
				FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
				convertedBitmap.BeginInit();
				convertedBitmap.Source = value;
				convertedBitmap.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
				convertedBitmap.EndInit();

				int stride = (int)convertedBitmap.PixelWidth * (convertedBitmap.Format.BitsPerPixel / 8);
				byte[] b = new byte[convertedBitmap.PixelHeight * stride];
				convertedBitmap.CopyPixels(b, stride, 0);
				_bitmap = new BitmapImage(); Bytes = b;
				BitmapSource source = BitmapSource.Create(convertedBitmap.PixelWidth, convertedBitmap.PixelHeight, convertedBitmap.DpiX, convertedBitmap.DpiY,
					convertedBitmap.Format, convertedBitmap.Palette, b, stride);
				_bitmap = source;
				Width = source.PixelWidth;
				Height = source.PixelHeight;

				OnPropertyChanged(nameof(Bitmap));
			}
		}
	}
}
