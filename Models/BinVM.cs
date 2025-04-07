using Photozhop.BinMethods;
using Photozhop.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Photozhop.Models
{
	class BinVM : BindHelper
	{
		private ImageModel _imageModel;
		private BitmapSource _image;
		private byte[] bytes;
		private byte[] out_bytes;
		private IBinaryzation selectedMethod;
		private ICommand doBinaryzation;
		private int _width;
		private int _height;
		//parameters for niblack
		private float _k;
		private int _radius;

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
			new GavrMethod(), new OtsuMethod(), new NiblackMethod(), new SauvolaMethod(), new WolfMethod()
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

		public float K
		{
			get => _k;
			set
			{
				_k = value;
				OnPropertyChanged(nameof(K));
			}
		}

		public int Radius
		{
			get => _radius;
			set
			{
				_radius = value;
				OnPropertyChanged(nameof(Radius));
			}
		}

		public ICommand DoBinaryzation
		{
			get
			{
				return doBinaryzation ??= new RelayCommand((t) => true, (_) =>
				{
					GrayScale();
					if(SelectedMethod is NiblackMethod)
					{
						((NiblackMethod)SelectedMethod).SetParams(10, 0.2f, _width,_height);
					}
					else if (SelectedMethod is SauvolaMethod)
					{
						((SauvolaMethod)SelectedMethod).SetParams(15, 0.2f, _width, _height);
					}
					else if (SelectedMethod is WolfMethod)
					{
						((WolfMethod)SelectedMethod).SetParams(10, _width, _height);
					}
						SelectedMethod.Binaryze(ref out_bytes);
					Image = BitmapSource.Create(_width, _height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null, out_bytes, _width * 4);
					Image.Freeze();
					out_bytes = Array.Empty<byte>();
				});
			}
		}

		public BinVM(ImageModel src)
		{
			this._imageModel = src;
			this.Image = src.Bitmap; this.bytes = src.Bytes;
			SelectedMethod = methods[0];
			OnPropertyChanged(nameof(SelectedMethod));
			this._height = src.Height;
			this._width = src.Width;
		}

		private void GrayScale()
		{
			out_bytes = new byte[bytes.Length];
			for (int i = 0; i < bytes.Length; i += 4)
			{
				out_bytes[i] = (byte)(0.2125 * bytes[i + 2] + 0.7154 * bytes[i + 1] + 0.0721 * bytes[i]) > 255 ? (byte)255 : (byte)(0.2125 * bytes[i + 2] + 0.7154 * bytes[i + 1] + 0.0721 * bytes[i]);
				out_bytes[i + 3] = bytes[i + 3];
			}

		}
	}
}
