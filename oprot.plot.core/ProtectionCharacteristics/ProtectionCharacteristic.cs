using MicroMvvm;
using Newtonsoft.Json;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace oprot.plot.core
{
    public abstract class ProtectionCharacteristic : GraphableFeature, IComparable
    {
        public double MaximumFaultLevel { get; set; } = double.PositiveInfinity;

        public int CompareTo(object obj)
        {
            int faster = 0;
            int equal = 0;
            int slower = 0;


            if (obj is ProtectionCharacteristic c)
            {
                int n = 1000;
                double logX0 = Math.Log10(1);
                double logX1 = Math.Log10(100000);
                double interval = (logX1 - logX0) / n;

                for (int i = 0; i < n; i++)
                {
                    double x = Math.Pow(10, logX0 + interval * i);
                    double y1 = Curve(x);
                    double y2 = c.Curve(x);
                    if (y1 == y2 || (double.IsPositiveInfinity(y1) && double.IsPositiveInfinity(y2)))
                    {
                        equal++;
                    }
                    else if (y1 < y2)
                    {
                        faster++;
                    }
                    else if (y1 > y2)
                    {
                        slower++;
                    }
                }
                if (slower == 0 && faster == 0)
                    return 0;
                if (slower > faster)
                    return -1;
                return 1;
            }
            else
            {
                throw new ArgumentException("Object is not a valid ProtectionCharacteristic");
            }
        }

        /// <summary>
        /// The equation for the curve as a function of current
        /// </summary>
        /// <param name="d">Current</param>
        /// <returns></returns>
        public abstract double Curve(double d);
        public abstract double LowerMargin(double d);
        public abstract double UpperMargin(double d); 
    }

    public abstract class FixedMarginCharacteristic : ProtectionCharacteristic
    {
        private double _discriminationMargin = 0.2;

        public double DiscriminationMargin
        {
            get
            {
                return _discriminationMargin;
            }
            set
            {
                if (value < 0.01 || value > 1)
                    return;
                _discriminationMargin = value;
                RaiseFeatureChanged();
            }
        }

        public bool ShowDiscriminationMargin { get; set; }

        public override double LowerMargin(double d)
        {
            return Math.Max(Curve(d) - DiscriminationMargin, 0);
        }

        public override double UpperMargin(double d)
        {
            return Curve(d) + DiscriminationMargin;
        }

        protected override PlotElement GetPlotElement()
        {
            var s = new LogFunctionSeries(Curve, PlotParameters.MinimumCurrent, PlotParameters.MaximumCurrent, PlotParameters.NumberOfSamples, DisplayName, DiscriminationMargin, TempMultiplier * PlotParameters.BaseVoltage / Voltage);
            s.ShowDiscriminationMargin = ShowDiscriminationMargin;
            s.Color = this.Color;
            if (TempMultiplier != 1.0)
                s.LineStyle = LineStyle.Dash;
            return s;
        }
    }

}
