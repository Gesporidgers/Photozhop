using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System;
using System.Windows.Media.Imaging;
using Photozhop.Utility;
using System.Threading.Tasks;

//using MathNet.Numerics.LinearAlgebra;


namespace Photozhop.Models
{
	class GradBrainVM : BindHelper
	{
		private ImageModel src;
		private BitmapSource _image;
		private Bitmap _histo = new Bitmap(256, 100);

		private byte[] _bytes;
		private PointF[] InterpolatedPoints;
		private InterpolatedInterval[] intervals;
		public ObservableCollection<PointF> points;
		public BitmapSource Image
		{
			get => _image;
			set
			{
				_image = value;
				OnPropertyChanged(nameof(Image));
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

		public Bitmap Histo
		{
			get => _histo;
		}

		public GradBrainVM(ref ImageModel src)
		{
			this.src = src; this.Image = src.Bitmap;
			this.Bytes = src.Bytes;
			points = new ObservableCollection<PointF>();
			points.Add(new PointF(0, 0));
			updateHisto();
			points.CollectionChanged += (s, e) =>
			{
				InterpolatePoints();
				UpdateImage();
				updateHisto();
			};
		}

		private void InterpolatePoints()        // Сделать костыль на добавление элемента
		{
			InterpolatedPoints = new PointF[100];
			List<PointF> lstPoints = points.ToList();
			lstPoints.Add(new PointF(1f, 1f));
			float[] Hs = new float[lstPoints.Count - 1];
			float[,] As = new float[Hs.Length - 1, 3];
			float[] Fs = new float[Hs.Length - 1];
			for (int i = 0; i < Hs.Length; i++)         // Maybe Parallel?
				Hs[i] = lstPoints[i + 1].X - lstPoints[i].X;
			for (int i = 1; i < Hs.Length; i++)     // Maybe Parallel?
			{
				As[i - 1, 0] = Hs[i];
				As[i - 1, 1] = 2 * (Hs[i - 1] + Hs[i]);
				As[i - 1, 2] = Hs[i];
				float y0 = lstPoints[i - 1].Y;
				float y1 = lstPoints[i].Y;
				float y2 = lstPoints[i + 1].Y;
				Fs[i - 1] = 6 * (((y2 - y1) / Hs[i]) - ((y1 - y0) / Hs[i - 1]));
			}
			As[0, 0] = 0;
			/*var A = Matrix<float>.Build.DenseOfArray(As);
			var f = Vector<float>.Build.Dense(Fs);

			var c = A.Solve(f).ToArray();*/
			float[] alpha = new float[Hs.Length];
			float[] beta = new float[Hs.Length];
			Array.Clear(alpha, 0, alpha.Length);
			Array.Clear(beta, 0, beta.Length);
			alpha[0] = As[0, 1]; beta[0] = Fs[0];
			if (Hs.Length > 2)
			{
				for (int i = 1; i < alpha.Length - 1; i++)
				{
					alpha[i] = As[i, 1] - (As[i, 0] * As[i - 1, 2]) / alpha[i - 1];
					beta[i] = Fs[i] - beta[i - 1] * (As[i, 0] / alpha[i - 1]);
				}
			}

			float[] c = new float[lstPoints.Count];


			if (beta.Last() == 0 || alpha.Last() == 0)
				c[c.Length - 1] = 0;
			else
				c[c.Length - 1] = beta[beta.Length - 2] / alpha[alpha.Length - 2];
			if (As.GetUpperBound(0) == 0)
				c[c.Length - 2] = (beta[0] - As[0, 2] * c[1]) / alpha[0];
			else
			{
				c[c.Length - 2] = beta[beta.Length - 2] / alpha[alpha.Length - 2];
				for (int i = c.Length - 3; i > 0; i--)
					c[i] = (beta[i - 1] - As[i - 1, 2] * c[i + 1]) / alpha[i - 1];
			}
			float[] a = (from p in lstPoints
						 where p != lstPoints.First()
						 select p.Y).ToArray();
			float[] _c = c.Skip(1).ToArray();
			float[] b = new float[lstPoints.Count - 1];
			float[] d = new float[lstPoints.Count - 1];
			for (int i = 0; i < b.Length; i++)
			{
				b[i] = (((lstPoints[i + 1].Y - lstPoints[i].Y) / Hs[i])) + (c[i + 1] * Hs[i] / 3f) + (c[i] * Hs[i] / 6);
				d[i] = (c[i + 1] - c[i]) / Hs[i];
			}
			intervals = (from i in Enumerable.Range(0, a.Length)
						 select new InterpolatedInterval
						 {
							 A = a[i],
							 B = b[i],
							 C = _c[i],
							 D = d[i],
							 start = lstPoints[i].X,
							 end = lstPoints[i + 1].X
						 }).ToArray();
			//intervals = (from aa in a
			//			 from bb in b
			//			 from cc in c
			//			 from dd in d
			//			 select new InterpolatedInterval
			//			 {
			//				 A = aa, B = bb, C = cc, D = dd
			//			 }).ToArray();
			//for (int i = 0; i < intervals.Length-1; i++)
			//{
			//	intervals[i].start = lstPoints[i].X;
			//	intervals[i].end = lstPoints[i + 1].X;
			//}
			uint j = 0; uint ii = 0;
			InterpolatedPoints = new PointF[256];
			for (float i = 0f; i < 1f; i += 0.00390625f)
			{
				if (intervals[j].InRange(i))
				{
					float tmpY = intervals[j].A + intervals[j].B * (i - intervals[j].end) + intervals[j].C * 0.5f * (i - intervals[j].end) * (i - intervals[j].end) + (intervals[j].D / 6f) * (i - intervals[j].end) * (i - intervals[j].end) * (i - intervals[j].end);
					InterpolatedPoints[ii] = new PointF(i, tmpY);
				}
				else
				{
					j++;
					float tmpY = intervals[j].A + intervals[j].B * (i - intervals[j].end) + intervals[j].C * 0.5f * (i - intervals[j].end) * (i - intervals[j].end) + (intervals[j].D / 6f) * (i - intervals[j].end) * (i - intervals[j].end) * (i - intervals[j].end);
					InterpolatedPoints[ii] = new PointF(i, tmpY);
				}

				ii++;
			}
		}

		public Point[] GetPoints(int k, bool invertY)
		{
			Point[] ps = new Point[InterpolatedPoints.Length];
			Parallel.For(0, ps.Length, (i) =>
			{
				ps[i] = new Point((int)(InterpolatedPoints[i].X * k), invertY ? (int)((1 - InterpolatedPoints[i].Y) * k) : (int)(InterpolatedPoints[i].Y * k));
				//if (ps[i].X == 0 && i != 0)
				//	throw new NotFiniteNumberException();
			});
			//for (int i = 0; i < ps.Length; i++)
			//{

			//}
			return ps;
		}

		public void UpdateImage()
		{
			Bytes = src.Bytes;
			Point[] newPixelInt = GetPoints(255, false);
			Parallel.For(0, Bytes.Length, (i) => { Bytes[i] = (byte)(newPixelInt[(int)Bytes[i]].Y); });
			//for (int i = 0; i < Bytes.Length; i++)

			Image = BitmapSource.Create(src.Width, src.Height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null, Bytes, src.Width * 4);
			src.Bitmap = Image;
		}

		public void updateHisto()
		{
			_histo = new Bitmap(256, 100);
			int[] pixelIntensity = new int[256];
			//for(int i = 0; i < 256; i++)
			Parallel.For(0, src.Width * src.Height, (i) =>
			{
				int y = i / src.Width;
				int x = i - y * src.Height;
				int _i = y * src.Width + x;
				if (x > 0 && x < src.Width && y > 0 && y < src.Height)
				{
					int avgInt = ((int)(Bytes[_i * 4]) + (int)(Bytes[_i * 4 + 1]) + (int)(Bytes[_i * 4 + 2])) / 3;
					pixelIntensity[avgInt]++;
				}

			});
			float k = 100f / pixelIntensity.Max();
			Graphics g = Graphics.FromImage(Histo);
			var p = Pens.Black.Clone() as System.Drawing.Pen;
			p.Width = 1;
			for (int i = 0; i < 256; i++)
				g.DrawLine(p, i, 99, i, 99 - pixelIntensity[i] * k);
			p.Dispose();
			g.Dispose(); Bytes = Array.Empty<byte>();
		}
	}
}