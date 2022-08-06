using CommunityToolkit.Mvvm.ComponentModel;

namespace oprot.plot.core
{
    public abstract partial class GenericInverseCharacteristic : FixedMarginCharacteristic
    {
        //TODO: make these global settings?
        protected double _maxTripTimeHardLimit = 1e6;
        protected double _minTripHardLimit = 0.01;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Description))]
        private double tms  = 1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Description))]
        private double pickup  = 100;

        [ObservableProperty]
        private double maxTripTime  = double.PositiveInfinity;

        [ObservableProperty]
        private double hiSetPickup  = double.PositiveInfinity;

        [ObservableProperty]
        private double minTripMultiplier = 1;
        
        /// <summary>
        /// Will copy settings from other GenericInverseCharacteristic curves
        /// </summary>
        /// <param name="f"></param>
        protected override void PretendCopyConstructor(ProtectionCharacteristic c)
        {
            base.PretendCopyConstructor(c);
            if (c is GenericInverseCharacteristic c2)
            {
                Tms = c2.Tms;
                Pickup = c2.Pickup;
                HiSetPickup = c2.HiSetPickup;
                MaxTripTime = c2.MaxTripTime;
                MinTripMultiplier = c2.MinTripMultiplier;
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
            if (d >= HiSetPickup)
                return 0.01;
            if (d < pickup*MinTripMultiplier)
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

        /*
        protected override PlotElement GetPlotElement()
        {
            var s = new LogFunctionSeries(Curve, PlotParameters.MinimumCurrent, PlotParameters.MaximumCurrent, PlotParameters.NumberOfSamples, DisplayName, DiscriminationMargin, TempMultiplier* PlotParameters.BaseVoltage/ Voltage);
            s.ShowDiscriminationMargin = ShowDiscriminationMargin;
            s.Color = this.Color;
            if (TempMultiplier != 1.0)
                s.LineStyle = LineStyle.Dash;
            return s;
        } 
        */
    }
}
