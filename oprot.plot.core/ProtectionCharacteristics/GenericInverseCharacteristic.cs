using System;
using MicroMvvm;
using OxyPlot;

namespace oprot.plot.core
{
    public abstract class GenericInverseCharacteristic : ProtectionCharacteristic
    {
        protected double _tms = 1;
        protected double _pickup = 100;
        protected double _maxTripTime = double.PositiveInfinity;
        protected double _maxTripTimeHardLimit = 1e6;
        protected double _hiSet = double.PositiveInfinity;
        protected double _minTripHardLimit = 0.01;
        protected double _minTripMultiplier = 1.0;

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
                RaiseGraphElementInvalidated();
            }
        }

        public double TMS
        {
            get
            {
                return _tms;
            }
            set
            {
                _tms = value;
                RaisePropertyChanged(nameof(TMS));
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

        public double MaxTripTime
        {
            get
            {
                return _maxTripTime;
            }
            set
            {
                _maxTripTime = value;
                RaisePropertyChanged(nameof(MaxTripTime));
                UpdateGraphElement();
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
                RaisePropertyChanged(nameof(HiSetPickup));
                UpdateGraphElement();
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
                RaisePropertyChanged(nameof(MinTripMultiplier));
                UpdateGraphElement();
            }
        }

        public GenericInverseCharacteristic() : base() { }

        public GenericInverseCharacteristic(GraphFeature g) : base(g) {
            if (g is GenericInverseCharacteristic g2)
            {
                _tms = g2.TMS;
                _pickup = g2.Pickup;
                _hiSet = g2.HiSetPickup;
                _maxTripTime = g2.MaxTripTime;
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

        public override PlotElement GetPlotElement()
        {
            var s = new LogFunctionSeries(Curve, _minimumCurrent, _maximumCurrent, _numberSamples, DisplayName, DiscriminationMargin, _tempMultiplier * _baseVoltage / _voltage);
            s.ShowDiscriminationMargin = ShowDiscriminationMargin;
            s.Color = _color;
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
