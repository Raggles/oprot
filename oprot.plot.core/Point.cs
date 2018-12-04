using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static Point Parse (string s)
        {
            string[] sa = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            
            return new Point { X = double.Parse(sa[0]), Y = double.Parse(sa[1]) };
        }
    }
}
