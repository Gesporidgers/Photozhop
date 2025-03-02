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
		public GradWindow(ImageModel sourceImage)
		{
			vm = new GradBrainVM { Image = sourceImage.Bitmap, Bytes = sourceImage.Bytes };
			DataContext = vm;
			InitializeComponent();
			pictureBox.Image = vm.curve;
			using Graphics g = Graphics.FromImage(vm.curve);

			var p = Pens.Black.Clone() as System.Drawing.Pen;
			p.Width = 1;

			g.DrawLine(p, 0, 0, 0, 500);
			g.DrawLine(p, 0, 499, 500, 499);

			pictureBox.Refresh(); p.Dispose();
		}

		private void CloseApply(object sender, RoutedEventArgs e)
		{
			this.Close();

		}

		private void pictureBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			using Graphics g = Graphics.FromImage(vm.curve);
			g.FillEllipse(System.Drawing.Brushes.Black, e.X, e.Y, 5, 5);
			pictureBox.Refresh();
		}


	}
}
