using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System;
using System.Windows.Media.Imaging;
using Photozhop.Utility;
using MathNet.Numerics.LinearAlgebra;


namespace Photozhop.Models
{
	class GradBrainVM
	{
		private ImageModel src;
		private BitmapSource _image;
		private Bitmap _histo;

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
			points.CollectionChanged += (s, e) => InterpolatePoints(); // Возможно надо убрать
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
				for (int i = 1; i < alpha.Length; i++)
				{
					alpha[i] = (As[i, 1] * As[i - 1, 2]) / alpha[i - 1];
					beta[i] = Fs[i] - beta[i - 1] * (As[i, 0] / alpha[i - 1]);
				}
			}

			float[] c = new float[lstPoints.Count];


			if (beta.Last() == 0 || alpha.Last() == 0)
				c[c.Length - 1] = 0;
			else
				c[c.Length - 1] = beta.Last() / alpha.Last();
			if (As.GetUpperBound(0) == 0)
				c[c.Length - 2] = 0;
			else
				for (int i = c.Length - 2; i > -1; i--)
					c[i] = (beta[i] - As[i, 2] * c[i + 1]) / alpha[i];
			float[] a = (from p in lstPoints
						 where p != lstPoints.First()
						 select p.Y).ToArray();
			float[] b = new float[lstPoints.Count - 1];
			float[] d = new float[lstPoints.Count - 1];
			for (int i = 1; i < b.Length; i++)
			{
				b[i] = (((lstPoints[i].Y - lstPoints[i - 1].Y) / Hs[i])) + (c[i] * Hs[i] / 3f) + (c[i - 1] * Hs[i] / 6);
				d[i] = (c[i] - c[i - 1]) / Hs[i];
			}
			intervals = (from i in Enumerable.Range(0, a.Length)
						 select new InterpolatedInterval
						 {
							 A = a[i],
							 B = b[i],
							 C = c[i],
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
			uint j = 0;
			InterpolatedPoints = new PointF[100];
			for (int i = 0; i < 100; i++)
			{
				if (intervals[j].InRange(i / 100f))
				{
					float tmpY = intervals[j].A + intervals[j].B * (i / 100f - intervals[j].start) + intervals[j].C * 0.5f * (i / 100f - intervals[j].start) * (i / 100f - intervals[j].start) + (intervals[j].D / 6f) * (i / 100f - intervals[j].start) * (i / 100f - intervals[j].start) * (i / 100f - intervals[j].start);
					InterpolatedPoints[i] = new PointF(i / 100f, tmpY);
				}
				else j++;

			}
		}

		public Point[] GetPoints(int k, bool invertY)
		{
			Point[] ps = new Point[InterpolatedPoints.Length];
			for (int i = 0; i < ps.Length; i++)
			{
				ps[i] = new Point((int)InterpolatedPoints[i].X * k, invertY ? (1 - (int)InterpolatedPoints[i].Y) * k : (int)InterpolatedPoints[i].Y * k);
			}
			return ps;
		}
	}
}