using OxyPlot;
using PropertyChanged;

namespace oprot.plot.core
{
    public class DefiniteTimeCharacteristic : FixedMarginCharacteristic
    {
        private double _time = 10;
        private double _pickup = 100;
        private double _hiset = double.PositiveInfinity;
        private readonly double _maxTripTimeHardLimit = 1e6;

        [AlsoNotifyFor(nameof(Description))]
        public double Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
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

        [AlsoNotifyFor(nameof(Description))]

        public double HiSetPickup
        {
            get
            {
                return _hiset;
            }
            set
            {
                _hiset = value;
                RaisePropertyChanged(nameof(HiSetPickup));
                RaiseFeatureChanged();
            }
        }

    
        protected override void PretendCopyConstructor(GraphableFeature f)
        {
            base.PretendCopyConstructor(f);
            if (f is DefiniteTimeCharacteristic f2)
            {
                _time = f2.Time;
                _pickup = f2.Pickup;
                _hiset = f2.HiSetPickup;
            }
        }
        
        public override double Curve(double d)
        {
            if (d >= _hiset)
                return 0.01;
            if (d >= _pickup)
            {
                return _time;
            }
            else
                return _maxTripTimeHardLimit;
        }

        public override string ToString()
        {
            return $"{Pickup}A@{Time} DT";
        }

        
        protected override PlotElement GetPlotElement()
        {
            var s = new LogFunctionSeries(Curve, PlotParameters.MinimumCurrent, PlotParameters.MaximumCurrent, PlotParameters.NumberOfSamples, DisplayName, DiscriminationMargin, TempMultiplier * PlotParameters.BaseVoltage / Voltage)
            {
                ShowDiscriminationMargin = ShowDiscriminationMargin,
                Color = this.Color
            };
            if (TempMultiplier != 1.0)
                s.LineStyle = LineStyle.Dash;
            return s;
        }

    }
}
