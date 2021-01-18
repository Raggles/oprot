using System;
using MicroMvvm;
using OxyPlot;
using PropertyChanged;

namespace oprot.plot.core
{
    public abstract class GenericInverseCharacteristic : FixedMarginCharacteristic
    {
        //TODO: make these global settings?
        protected double _maxTripTimeHardLimit = 1e6;
        protected double _minTripHardLimit = 0.01;

        [AlsoNotifyFor(nameof(Description), nameof(DisplayName))]
        public double TMS { get; set; } = 1;

        [AlsoNotifyFor(nameof(Description), nameof(DisplayName))]
        public double Pickup { get; set; } = 100;

        public double MaxTripTime { get; set; } = double.PositiveInfinity;

        public double HiSetPickup { get; set; } = double.PositiveInfinity;

        public double MinTripMultiplier { get; set; } = 1;
        
        /// <summary>
        /// Will copy settings from other GenericInverseCharacteristic curves
        /// </summary>
        /// <param name="f"></param>
        protected override void PretendCopyConstructor(GraphableFeature f)
        {
            base.PretendCopyConstructor(f);
            if (f is GenericInverseCharacteristic c2)
            {
                TMS = c2.TMS;
                Pickup = c2.Pickup;
                HiSetPickup = c2.HiSetPickup;
                MaxTripTime = c2.MaxTripTime;
                MinTripMultiplier = c2.MinTripMultiplier;
            }
            RaiseFeatureChanged();
        }
        
        /// <summary>
        /// The curve equation for the inverse curve represented by the child class
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        protected abstract double CurveEquation(double d);

        /// <summary>
        /// This handles things like instantenous trip settings, trip multipliers etc
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public override double Curve(double d)
        {
            if (d >= HiSetPickup)
                return 0.01;
            if (d < Pickup*MinTripMultiplier)
                return _maxTripTimeHardLimit;
            double tripTime = CurveEquation(d);
            if (tripTime > MaxTripTime)
                return MaxTripTime;
            if (tripTime > _maxTripTimeHardLimit)
                return _maxTripTimeHardLimit;
            if (tripTime < _minTripHardLimit)
                return _minTripHardLimit;
            return tripTime;
        }

        
        protected override PlotElement GetPlotElement()
        {
            var s = new LogFunctionSeries(Curve, PlotParameters.MinimumCurrent, PlotParameters.MaximumCurrent, PlotParameters.NumberOfSamples, DisplayName, DiscriminationMargin, TempMultiplier* PlotParameters.BaseVoltage/ Voltage);
            s.ShowDiscriminationMargin = ShowDiscriminationMargin;
            s.Color = this.Color;
            if (TempMultiplier != 1.0)
                s.LineStyle = LineStyle.Dash;
            return s;
        }     
    }
}
