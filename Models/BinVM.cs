using Photozhop.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Photozhop.Models
{
	internal class BinVM : BindHelper
	{
		private ImageModel _imageModel;
		private BitmapSource _image;

		public BitmapSource Image
		{
			get => _image;
			set
			{
				_image = value;
				OnPropertyChanged(nameof(Image));
			}
		}

		public BinVM(ref ImageModel src)
		{
			this._imageModel = src;
			this.Image = src.Bitmap;
		}
	}
}
