using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photozhop.Utility
{
	public class BindHelper : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string Name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
		}
	}
}
