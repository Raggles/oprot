using System;
using System.Windows;

namespace oprot.plot.core
{
    public class DataInterpolator
    {
        private Point[] _points;
        bool ascending = false;
        private double _maxTimeHardLimit = 1e6;
        private double _minTimeHardLimit = 0.01;
        private bool _logMode = true;

        public DataInterpolator(Point[] points, bool log = true)
        {
            if (IsSorted(points))
            {
                _points = points;
                ascending = points[1].X - points[0].X > 0;
            }
            else
            {
                throw (new ArgumentException("Data must be sorted"));
            }
            _logMode = log;
        }

        /// <summary>
        /// Check that the list is sorted
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool IsSorted(Point[] list)
        {
            if (list.Length < 2) return false;
            bool xascending = list[1].X - list[0].X > 0;
            bool yascending = list[1].Y - list[0].Y > 0;

            for (int i = 0; i < list.Length - 1; i++)
            {
                if (xascending)
                {
                    if (list[i+1].X < list[i].X)
                        return false;
                }
                else
                {
                    if (list[i+1].X > list[i].X)
                        return false;
                }            
                if (yascending)
                {
                    if (list[i + 1].Y < list[i].Y)
                        return false;
                }
                else
                {
                    if (list[i + 1].Y > list[i].Y)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Interpolate the value for a point
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Interpolate(double x)
        {
            int x1 = 0;
            int x2 = 0;

            if (ascending)
            {
                if (x < _points[0].X)
                    return _maxTimeHardLimit;
                if (x > _points[_points.Length - 1].X)
                    return _minTimeHardLimit;

                for (int i = 0; i < _points.Length; i++)
                {
                    if (x > _points[i].X)
                        continue;
                    if (x == _points[i].X)
                        return _points[i].Y;
                    if (x < _points[i].X)
                    {
                        x1 = i - 1;
                        x2 = i;
                        break;
                    }
                }
            }
            else
            {
                if (x > _points[0].X)
                    return _minTimeHardLimit;
                if (x < _points[_points.Length - 1].X)
                    return _maxTimeHardLimit;

                for (int i = 0; i < _points.Length; i++)
                {
                    if (x < _points[i].X)
                        continue;
                    if (x == _points[i].X)
                        return _points[i].Y;
                    if (x > _points[i].X)
                    {
                        x1 = i - 1;
                        x2 = i;
                        break;
                    }
                }
            }
            if (_logMode)
            {
                double slope = (Math.Log10(_points[x2].Y) - Math.Log10(_points[x1].Y)) / (Math.Log10(_points[x2].X) - Math.Log10(_points[x1].X));
                return Math.Pow(10, (Math.Log10(x) - Math.Log10(_points[x1].X)) * slope + Math.Log10(_points[x1].Y));
            }
            else
            {
                double slope = (_points[x2].Y - _points[x1].Y) / (_points[x2].X - _points[x1].X);
                return (x - _points[x1].X) * slope + _points[x1].Y;
            }
        }
    }
}
