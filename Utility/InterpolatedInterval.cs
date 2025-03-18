using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photozhop.Utility
{
    struct InterpolatedInterval
    {
        public float A;
        public float B;
        public float C;
        public float D;
        public float start;
        public float end;
        public bool InRange(float x) => x >= start && x <= end;
        
    }
}
