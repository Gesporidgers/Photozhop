using Photozhop.Models;
using System;
using System.Collections.Generic;
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
	/// Логика взаимодействия для BinarizationWindow.xaml
	/// </summary>
	public partial class BinarizationWindow : Window
	{
		private BinVM vm;
		public BinarizationWindow(ref ImageModel image)
		{
			vm = new BinVM(ref image);
			DataContext = vm;
			InitializeComponent();
		}
		private void CloseApply(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
