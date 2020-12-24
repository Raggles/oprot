using System;
using MicroMvvm;
using OxyPlot;
using PropertyChanged;

namespace oprot.plot.core
{
    public abstract class GenericInverseCharacteristic : FixedMarginCharacteristic
    {
        protected double _tms = 1;
        protected double _pickup = 100;
        protected double _maxTripTime = double.PositiveInfinity;
        protected double _maxTripTimeHardLimit = 1e6;
        protected double _hiSet = double.PositiveInfinity;
        protected double _minTripHardLimit = 0.01;
        protected double _minTripMultiplier = 1.0;

        [AlsoNotifyFor(nameof(Description))]
        public double TMS
        {
            get
            {
                return _tms;
            }
            set
            {
                _tms = value;
                RaiseFeatureChanged();
            }
        }

        [AlsoNotifyFor(nameof(Description))]
        public double Pickup
        {
            get
            {
                return _pickup;
            }
            set
            {
                _pickup = value;
                RaiseFeatureChanged();
            }
        }

        public double MaxTripTime
        {
            get
            {
                return _maxTripTime;
            }
            set
            {
                _maxTripTime = value;
                RaiseFeatureChanged();
            }
        }

        public double HiSetPickup
        {
            get
            {
                return _hiSet;
            }
            set
            {
                _hiSet = value;
                RaiseFeatureChanged();
            }
        }

        public double MinTripMultiplier
        {
            get
            {
                return _minTripMultiplier;
            }
            set
            {
                _minTripMultiplier = value;
                RaiseFeatureChanged();
            }
        }

        public GenericInverseCharacteristic() : base() { }

        protected override void PretendCopyConstructor(GraphableFeature f)
        {
            base.PretendCopyConstructor(f);
            if (f is GenericInverseCharacteristic c2)
            {
                _tms = c2.TMS;
                _pickup = c2.Pickup;
                _hiSet = c2.HiSetPickup;
                _maxTripTime = c2.MaxTripTime;
            }
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
            if (d >= _hiSet)
                return 0.01;
            if (d < _pickup*_minTripMultiplier)
                return _maxTripTimeHardLimit;
            double tripTime = CurveEquation(d);
            if (tripTime > _maxTripTime)
                return _maxTripTime;
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
