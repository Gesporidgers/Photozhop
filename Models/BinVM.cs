using Photozhop.BinMethods;
using Photozhop.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Photozhop.Models
{
	class BinVM : BindHelper
	{
		//private ImageModel _imageModel;
		private BitmapSource _image;
		public byte[] bytes;
		private IBinaryzation selectedMethod;
		private ICommand doBinaryzation;
		private int _width;
		private int _height;
		public IBinaryzation SelectedMethod
		{
			get => selectedMethod;
			set
			{
				selectedMethod = value;
				OnPropertyChanged(nameof(SelectedMethod));
			}
		}

		public List<IBinaryzation> methods => new List<IBinaryzation>
		{
			new GavrMethod(), new OtsuMethod()
		};

		public BitmapSource Image
		{
			get => _image;
			set
			{
				_image = value;
				OnPropertyChanged(nameof(Image));
			}
		}

		public ICommand DoBinaryzation
		{
			get
			{
				return doBinaryzation ??= new RelayCommand((t) => true, (_) =>
				{
					GrayScale();
					SelectedMethod.Binaryze(ref bytes);
					Image = BitmapSource.Create(_width, _height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null, bytes, _width * 4);
					Image.Freeze();
					bytes = Array.Empty<byte>();
				});
			}
		}

		public BinVM(ImageModel src)
		{
			//this._imageModel = src;
			this.Image = src.Bitmap; this.bytes = src.Bytes;
			SelectedMethod = methods[0];
			OnPropertyChanged(nameof(SelectedMethod));
			this._height = src.Height;
			this._width = src.Width;
		}

		private void GrayScale()
		{
			for (int i = 0; i < bytes.Length; i += 4)
				bytes[i] = (byte)(0.2125 * bytes[i + 2] + 0.7154 * bytes[i + 1] + 0.0721 * bytes[i]) > 255 ? (byte)255 : (byte)(0.2125 * bytes[i + 2] + 0.7154 * bytes[i + 1] + 0.0721 * bytes[i]);

		}
	}
}
