using Photozhop.Models;
using System.Windows;
using System.Windows.Controls;

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
			if (ii - 1 <= 0)
			{
				vm.Bitmaps.Move(ii, ii - 1);
			}
			else
			{
				_ = MessageBox.Show("Невозможно переместить слой! Слой уже находится на верхнем положении", "Ошибка перемещения", MessageBoxButton.OK, MessageBoxImage.Error);
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
				_ = MessageBox.Show("Невозможно переместить слой! Слой уже находится на нижнем положении", "Ошибка перемещения", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			var tmp = e.Source as MenuItem;
			var par = tmp.DataContext as ImageModel;
			_ = vm.Bitmaps.Remove(par);
		}

		private void OpenGrad(object sender, RoutedEventArgs e)
		{
			var tmp = e.Source as MenuItem;
			var par = tmp.DataContext as ImageModel;
			var ii = vm.Bitmaps.IndexOf(par);
			GradWindow gradWindow = new GradWindow(ref par);
			if ((bool)gradWindow.ShowDialog())
				vm.Bitmaps[ii] = par;

		}

		private void OpenBin(object sender, RoutedEventArgs e)
		{
			var tmp = e.Source as MenuItem;
			var par = tmp.DataContext as ImageModel;
			var ii = vm.Bitmaps.IndexOf(par);
			BinarizationWindow binarizationWindow = new BinarizationWindow(ref par);
			if ((bool)binarizationWindow.ShowDialog())
				vm.Bitmaps[ii] = par;
		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			MatrixWindow matrixWindow = new MatrixWindow();
			matrixWindow.Show();
		}
	}
}
