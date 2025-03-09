using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		private ImageModel src;
		private BitmapSource _image;
		private Bitmap _histo;

		private byte[] _bytes;
		public ObservableCollection<PointF> points;
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

		public Bitmap Histogram
		{
			get => _histo;
		}

		public GradBrainVM(ref ImageModel src)
		{
			this.src = src; this.Image = src.Bitmap;
			this.Bytes = src.Bytes;
			points = new ObservableCollection<PointF>();
			points.Add(new PointF(0, 0));
			InterpolatePoints();
			points.CollectionChanged += (s, e) => InterpolatePoints();
		}

		private void InterpolatePoints()
		{
			
		}

		public PointF[] GetPoints()
		{
			PointF[] ps = new PointF[points.Count+1];
			for(int i  = 0; i < ps.Length-1; i++) 
				ps[i] =new PointF(points[i].X*500f,(1f-points[i].Y)*500f);
			ps[points.Count] = new PointF(500f,0f);
			return ps;
		}
	}
}