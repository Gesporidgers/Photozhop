using Microsoft.Win32;
using Photozhop.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Photozhop.Models
{
	class BrainVM : BindHelper
	{
		private BitmapSource _canvas;
		byte[] bytes = Array.Empty<byte>();
		private ICommand moveUpCommand;
		private ICommand moveDownCommand;
		private ICommand deleteCommand;
		private ICommand saveCommand;
		private ICommand openCommand;
		private TaskQueue taskQueue = new TaskQueue();


		public ObservableCollection<ImageModel> Bitmaps { get; set; }

		public List<PPO> operations => PPO.getPPO();

		public BitmapSource ResultImage
		{
			get => _canvas;
			set
			{
				_canvas = value;
				OnPropertyChanged(nameof(ResultImage));
			}
		}

		//public ICommand MoveUpCommand
		//{
		//	get
		//	{
		//		return moveUpCommand ??= new RelayCommand(_canMoveUp, (obj)=> {
		//			var par = obj as ImageModel;
		//			var ii = Bitmaps.IndexOf(par);
		//			Bitmaps.Move(ii, ii - 1);
		//		});
		//	}
		//}

		//public ICommand DeleteCommand
		//{
		//	get
		//	{
		//		return deleteCommand = new RelayCommand(t => true, (obj) => { Bitmaps.Remove(obj as ImageModel); });
		//	}
		//}

		//public ICommand MoveDownCommand
		//{
		//	get
		//	{
		//		return moveDownCommand = new RelayCommand(_canMoveDown, (obj) => {
		//			var par = obj as ImageModel;
		//			var ii = Bitmaps.IndexOf(par);
		//			Bitmaps.Move(ii, ii + 1);
		//		});
		//	}
		//}

		public ICommand OpenCommand
		{
			get
			{
				return openCommand ??= new RelayCommand(t => true, (obj) =>
				{
					OpenFileDialog openFileDialog = new OpenFileDialog();
					openFileDialog.Filter = "Изображения (*png; *jpg)|*png; *jpg";
					if (openFileDialog.ShowDialog() == true)
					{
						AddImage(new ImageModel { Bitmap = new BitmapImage(new Uri(openFileDialog.FileName)), Name = openFileDialog.SafeFileName });
					}
				});
			}
		}

		public ICommand SaveCommand
		{
			get
			{
				return saveCommand ??= new RelayCommand(t => Bitmaps.Count != 0, (obj) =>
				{
					SaveFileDialog fileDialog = new SaveFileDialog();
					fileDialog.DefaultExt = ".png";
					fileDialog.Filter = "Изображения (*png)|*png";
					if ((bool)fileDialog.ShowDialog())
					{
						using(var fs = new FileStream(fileDialog.FileName, FileMode.Create))
						{
							BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
							bitmapEncoder.Frames.Add(BitmapFrame.Create(ResultImage));
							bitmapEncoder.Save(fs);
						}
					}
				});
			}
		}

		public void AddImage(ImageModel img)
		{
			img.PropertyChanged += (s, prop_name) =>
			{
				if (prop_name.PropertyName == nameof(ImageModel.Opacity) ||
					prop_name.PropertyName == nameof(ImageModel.SelectedOperation))
				{
					taskQueue.AddTask(new Task(CalcLayers));
				}
			};
			Task.Delay(1);
			Bitmaps.Add(img);
		}

		public void updateCanvas()
		{
			int max_width = Bitmaps.Max(i => i.Width);
			int max_height = Bitmaps.Max(i => i.Height);
			_canvas = new WriteableBitmap(max_width, max_height, 96, 96, PixelFormats.Bgra32, null);
		}

		public BrainVM()
		{
			taskQueue.StartQueue();
			Bitmaps = new ObservableCollection<ImageModel>();
			//_canMoveDown = (obj) => Bitmaps.LastOrDefault() != (obj as ImageModel);
			//_canMoveUp = (obj) => Bitmaps.FirstOrDefault() != (obj as ImageModel);
			Bitmaps.CollectionChanged += (s, e) =>
			{
				if (Bitmaps.Count > 0)
				{
					updateCanvas();
					taskQueue.AddTask(new Task(CalcLayers));
				}
				else
				{
					ResultImage = null;
				}
			};
		}

		public void CalcLayers()
		{
			var props = (from b in Bitmaps
						 select new
						 {
							 op = b.Opacity/100,
							 w = b.Width,
							 h = b.Height,
							 so = b.SelectedOperation
						 }).ToArray();
			int max_width = Bitmaps.Max(i => i.Width);
			int max_height = Bitmaps.Max(i => i.Height);
			if (bytes.Length != max_height * max_width * 4)
				bytes = new byte[max_height * max_width * 4];
			else
				Array.Clear(bytes, 0, bytes.Length);

			//for (int i = 0; i<max_height*max_width;++i)
			Parallel.For(0, max_height * max_width, (i) =>
			{
				int y = i / max_width;
				int x = i - y * max_width;
				int _i = y * props[0].w + x;

				for (int j = 0; j < props.Length; j++)
				{
					y = i / max_width;
					x = i - y * max_width;
					_i = y * props[j].w + x;

					if (x > 0 && x < props[j].w && y > 0 && y < props[j].h)
					{
						for (int k = 0; k < 4; k++)
						{
							byte tnp = (byte)(props[j].so.ByteOperation(Bitmaps[j].Bytes[_i * 4 + k], bytes[i * 4 + k]));
							bytes[i * 4 + k] = (byte)((byte)(tnp * props[j].op) + (byte)(bytes[i * 4 + k] * (1 - props[j].op)));
						}
					}

				}
			});
			BitmapSource res = BitmapSource.Create(max_width, max_height, 96, 96, PixelFormats.Bgra32, null, bytes, max_width * 4);
			bytes = Array.Empty<byte>();
			res.Freeze();
			ResultImage = res;
		}
	}
}
