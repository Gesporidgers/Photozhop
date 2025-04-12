using Photozhop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photozhop.Models
{
    class MatrixTransformVM : BindHelper
    {
        private float[,] _array = new float[3, 3];

        public float[,] Array
        {
            get => _array;
            set
            {
                _array = value;
                OnPropertyChanged(nameof(Array));
            }
        }
    }
}
