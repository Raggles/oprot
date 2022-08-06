using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace oprot.plot.core
{
    public abstract partial class FixedMarginCharacteristic : ProtectionCharacteristic
    {
        [ObservableProperty]
        private double discriminationMargin = 0.2;

        //TODO: remove
        //public bool ShowDiscriminationMargin { get; set; }

        public override double LowerMargin(double d)
        {
            return Math.Max(Curve(d) - DiscriminationMargin, 0);
        }

        public override double UpperMargin(double d)
        {
            return Curve(d) + DiscriminationMargin;
        }

        //TODO: remove
        /*
        protected override PlotElement GetPlotElement()
        {
            var s = new LogFunctionSeries(Curve, PlotParameters.MinimumCurrent, PlotParameters.MaximumCurrent, PlotParameters.NumberOfSamples, DisplayName, DiscriminationMargin, TempMultiplier * PlotParameters.BaseVoltage / Voltage);
            s.ShowDiscriminationMargin = ShowDiscriminationMargin;
            s.Color = this.Color;
            if (TempMultiplier != 1.0)
                s.LineStyle = LineStyle.Dash;
            return s;
        }
        */
    }

}
