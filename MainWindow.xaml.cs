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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Photozhop.Models;

namespace Photozhop
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private BrainVM vm;
		public MainWindow()
		{
			vm = new BrainVM();
			DataContext = vm;
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void MoveUp(object sender, RoutedEventArgs e)
		{
			var tmp = e.Source as MenuItem;
			var par = tmp.DataContext as ImageModel;
			var ii = vm.Bitmaps.IndexOf(par);
			if (ii - 1 >= 0)
			{
				vm.Bitmaps.Move(ii, ii - 1);
			}
			else
			{
				MessageBox.Show("Невозможно переместить слой! Слой уже находится на верхнем положении", "Ошибка перемещения", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private void MoveDown(object sender, RoutedEventArgs e)
		{
			var tmp = e.Source as MenuItem;
			var par = tmp.DataContext as ImageModel;
			var ii = vm.Bitmaps.IndexOf(par);
			if (ii + 1 < vm.Bitmaps.Count)
			{
				vm.Bitmaps.Move(ii, ii + 1);
			}
			else
			{
				MessageBox.Show("Невозможно переместить слой! Слой уже находится на нижнем положении", "Ошибка перемещения", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			var tmp = e.Source as MenuItem;
			var par = tmp.DataContext as ImageModel;
			vm.Bitmaps.Remove(par);
		}

		private void OpenGrad(object sender, RoutedEventArgs e)
		{
			var tmp = e.Source as MenuItem;
			var par = tmp.DataContext as ImageModel;
			//var ii = vm.Bitmaps.IndexOf(par);
			GradWindow gradWindow = new GradWindow(ref par);
			gradWindow.ShowDialog();
		}
	}
}
