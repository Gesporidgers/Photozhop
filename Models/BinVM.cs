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
using System.Windows;
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
		private Visibility _visibility;
		private Visibility _warningVisibility;

		public IBinaryzation SelectedMethod
		{
			get => selectedMethod;
			set
			{
				selectedMethod = value;
				if (SelectedMethod is NiblackMethod || SelectedMethod is SauvolaMethod || SelectedMethod is WolfMethod)
					VisibilityParams = Visibility.Visible;
				else
					VisibilityParams = Visibility.Collapsed;
				if (!(SelectedMethod is WolfMethod) || (_width < 2000 || _height < 2000))
					WarningVisibility = Visibility.Collapsed;
				else
					WarningVisibility = Visibility.Visible;
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

		public int K
		{
			get => (int)(_k * 10);
			set
			{
				_k = value / 10f;
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

		public Visibility VisibilityParams
		{
			get => _visibility;
			set
			{
				_visibility = value;
				OnPropertyChanged(nameof(VisibilityParams));
			}
		}

		public Visibility WarningVisibility
		{
			get => _warningVisibility;
			set
			{
				_warningVisibility = value;
				OnPropertyChanged(nameof(WarningVisibility));
			}
		}

		public ICommand DoBinaryzation
		{
			get
			{
				return doBinaryzation ??= new RelayCommand((t) => !(SelectedMethod is WolfMethod) || (_width < 2000 || _height < 2000), (_) =>
				{
					GrayScale();
					if (SelectedMethod is NiblackMethod)
					{
						((NiblackMethod)SelectedMethod).SetParams(Radius, _k, _width, _height);
					}
					else if (SelectedMethod is SauvolaMethod)
					{
						((SauvolaMethod)SelectedMethod).SetParams(Radius, _k, _width, _height);
					}
					else if (SelectedMethod is WolfMethod)
					{
						((WolfMethod)SelectedMethod).SetParams(Radius, _width, _height);
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
			this.K = 2; this.Radius = 2;
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
