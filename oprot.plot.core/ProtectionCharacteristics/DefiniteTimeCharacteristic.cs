using OxyPlot;

namespace oprot.plot.core
{
    public class DefiniteTimeCharacteristic : ProtectionCharacteristic
    {
        private double _time = 10;
        private double _pickup = 100;
        private double _hiset = double.PositiveInfinity;
        private readonly double _maxTripTimeHardLimit = 1e6;

        public double Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                RaisePropertyChanged(nameof(Time));
                RaisePropertyChanged(nameof(Description));
                UpdateGraphElement();
            }
        }

        public double Pickup
        {
            get
            {
                return _pickup;
            }
            set
            {
                _pickup = value;
                RaisePropertyChanged(nameof(Pickup));
                RaisePropertyChanged(nameof(Description));
                UpdateGraphElement();
            }
        }

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
                UpdateGraphElement();
            }
        }

        public override OxyColor Color
        {
            get
            {
                return _plotElement == null ? _color : ((LogFunctionSeries)_plotElement).ActualColor;
            }
            set
            {
                _color = value;
                if (_plotElement != null)
                    ((LogFunctionSeries)_plotElement).Color = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DisplayColor));
                RaiseGraphElementInvalidated();
            }
        }

        public DefiniteTimeCharacteristic() : base() { }
        public DefiniteTimeCharacteristic(GraphFeature g) : base(g)
        {
            if (g is DefiniteTimeCharacteristic g2)
            {
                _time = g2.Time;
                _pickup = g2.Pickup;
                _hiset = g2.HiSetPickup;
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
            return $" ({Pickup}A@{Time} DT)";
        }

        public override PlotElement GetPlotElement()
        {
            var s = new LogFunctionSeries(Curve, _minimumCurrent, _maximumCurrent, _numberSamples, DisplayName, DiscriminationMargin, _tempMultiplier * _baseVoltage / _voltage)
            {
                ShowDiscriminationMargin = ShowDiscriminationMargin,
                Color = _color
            };
            if (_tempMultiplier != 1.0)
                s.LineStyle = LineStyle.Dash;
            return s;
        }

        private bool _showDiscriminationMargin = true;
        private double _discriminationMargin = 0.2;

        public bool ShowDiscriminationMargin
        {
            get
            {
                return _showDiscriminationMargin;
            }
            set
            {
                _showDiscriminationMargin = value;
                ((LogFunctionSeries)_plotElement).ShowDiscriminationMargin = value;
                RaiseGraphElementInvalidated();
                RaisePropertyChanged(nameof(ShowDiscriminationMargin));
            }
        }

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
                RaisePropertyChanged(nameof(DiscriminationMargin));
                UpdateGraphElement();
            }
        }
    }
}
