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
    public abstract class ProtectionCharacteristic : GraphableFeature
    {
        public double MaximumFaultLevel { get; set; } = double.PositiveInfinity;

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
