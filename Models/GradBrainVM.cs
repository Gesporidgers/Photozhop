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
		private PointF[] InterpolatedPoints;
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
			points.CollectionChanged += (s, e) => InterpolatePoints(); // Возможно надо убрать
		}

		private void InterpolatePoints()		// Сделать костыль на добавление элемента
		{
			InterpolatedPoints = new PointF[100];
			List<PointF> lstPoints = points.ToList();
			lstPoints.Add(new PointF(1f, 1f));
			float[] Hs = new float[lstPoints.Count];
			float[,] As = new float[Hs.Length - 1, 3];
			float[] Fs = new float[Hs.Length - 1];
			for (int i = 0; i < Hs.Length; i++)
				Hs[i] = lstPoints[i + 1].X - lstPoints[i].X;
			for (int i = 1; i < Hs.Length - 1; i++)
			{
				As[i, 0] = Hs[i];
				As[i, 1] = 2 * (Hs[i] + Hs[i + 1]);
				As[i, 2] = Hs[i + 1];
				float y0 = lstPoints[i - 1].Y;
				float y1 = lstPoints[i].Y;
				float y2 = lstPoints[i + 1].Y;
				Fs[i] = 6 * (((y2 - y1) / Hs[i + 1]) - ((y1 - y0) / Hs[i]));
			}
		}

		public Point[] GetPoints(int k)
		{
			Point[] ps = new Point[InterpolatedPoints.Length];
			for (int i = 0; i < InterpolatedPoints.Length; i++)
			{
				ps[i] = new Point((int)(InterpolatedPoints[i].X * k), (int)(InterpolatedPoints[i].Y * k));
			}
			return ps;
		}
	}
}