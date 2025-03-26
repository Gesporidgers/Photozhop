using Photozhop.BinMethods;
using Photozhop.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Photozhop.Models
{
	class BinVM : BindHelper
	{
		private ImageModel _imageModel;
		private BitmapSource _image;
		private IBinaryzation selectedMethod;

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

		public BinVM(ImageModel src)
		{
			this._imageModel = src;
			this.Image = src.Bitmap;
			SelectedMethod = methods[0];
		}
	}
}
