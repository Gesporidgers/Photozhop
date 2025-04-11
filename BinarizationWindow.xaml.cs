using Photozhop.Models;
using System.Windows;

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
		private void Close(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			this.Close();
		}

		private void CloseApply(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			vm.ApplyData();
			this.Close();
		}
	}
}
