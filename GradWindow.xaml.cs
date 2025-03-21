using Photozhop.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Photozhop
{
    /// <summary>
    /// Логика взаимодействия для GradWindow.xaml
    /// </summary>
    public partial class GradWindow : Window
    {
		private GradBrainVM vm;
		private Bitmap curve = new Bitmap(512, 512);
		
		public GradWindow(ref ImageModel sourceImage)
		{
			vm = new GradBrainVM (ref sourceImage);
			DataContext = vm;
			InitializeComponent();
			pictureBox.Image = curve;
			hist.Image = vm.Histo;
			using Graphics g = Graphics.FromImage(curve);

			var p = Pens.Black.Clone() as System.Drawing.Pen;
			p.Width = 1;

			g.DrawLine(p, 0, 0, 0, 500);      // y
			g.DrawLine(p, 0, 499, 500, 499); // x
			g.DrawLine(p, 0, 512,512,0);
			
			pictureBox.Refresh(); p.Dispose(); hist.Refresh();
		}

		private void CloseApply(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void pictureBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			vm.points.Add(new PointF(e.X / 512f, (512f -e.Y) / 512f));
			curve.Dispose();
			curve = new Bitmap(512, 512);
			Graphics g = Graphics.FromImage(curve);
			
			Redraw(ref g);
			g.Dispose();
			pictureBox.Image = curve;
			pictureBox.Refresh();
			hist.Image = vm.Histo;
			hist.Refresh();
		}

		private void Redraw(ref Graphics g)
		{
			var p = Pens.Black.Clone() as System.Drawing.Pen;
			var pEllipse = Pens.Black.Clone() as System.Drawing.Pen;
			p.Width = 1;
			pEllipse.Width = 2;
			g.DrawLine(p, 0, 0, 0, 500);      // y
			g.DrawLine(p, 0, 499, 500, 499); // x
			g.DrawLines(p, vm.GetPoints(500,true));
			foreach (var point in vm.points) 
			g.DrawEllipse(pEllipse, point.X*500, 500-point.Y*500, 5, 5);
			p.Dispose(); pEllipse.Dispose();

		}

	}
}
