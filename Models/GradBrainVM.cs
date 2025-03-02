using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Photozhop.Models
{
		class GradBrainVM
		{
			private BitmapSource _image;
			public Bitmap curve = new Bitmap(500, 500);
			private byte[] _bytes;

			public BitmapSource Image
			{
				get => _image;
				set
				{
					_image = value;
				}
			}

			public byte[] Bytes
			{
				get => _bytes;
				set
				{
					_bytes = value;
				}
			}

		}
	}