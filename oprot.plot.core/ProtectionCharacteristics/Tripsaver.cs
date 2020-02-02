using System;
using System.Collections.Generic;
using System.Text;
using OxyPlot;

namespace oprot.plot.core
{
    public enum TripSaverFuse
    {
        _6T,
        _10T,
        _20T,
        _30T,
        _40T,
        _50T,
        _65T,
        _80T,
        _100T
    }

    public class TripSaver : ProtectionCharacteristic
    {
        private TripSaverFuse _fuseenum;
        private double _hiSet = double.PositiveInfinity;
        private double _maxTripTime = 2.0;
        private double _minTripMultiplier = 3.0;
        private double _maxTripTimeHardLimit = 1e6;
        private double _minTripHardLimit = 0.01;

        public TripSaverFuse Fuse
        {
            get { return _fuseenum; }
            set
            {
                _fuseenum = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
                UpdateGraphElement();
            }
        }

        public double HiSetMul
        {
            get
            {
                return _hiSet;
            }
            set
            {
                _hiSet = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
                UpdateGraphElement();
            }
        }

        public double Pickup
        {
            get
            {
                string r = _fuseenum.ToString().TrimEnd(new char[] { 'T', 'K' }).TrimStart(new char[] { '_' });
                return double.Parse(r);
            }
        }

        private double A
        {
            get
            {
                switch (_fuseenum) {
                    case TripSaverFuse._6T:
                        return 10.248;
                    case TripSaverFuse._10T:
                        return 12.6625;
                    case TripSaverFuse._20T:
                        return 13.4962;
                    case TripSaverFuse._30T:
                        return 13.1706;
                    case TripSaverFuse._40T:
                        return 13.5054;
                    case TripSaverFuse._50T:
                        return 13.31;
                    case TripSaverFuse._65T:
                        return 13.3188;
                    case TripSaverFuse._80T:
                        return 13.7059;
                    case TripSaverFuse._100T:
                        return 15.0494;
                    default:
                        return 10;
                }

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

        public TripSaver(GraphFeature g = null) : base(g)
        {
            if (g is TripSaver g2)
            {
                _fuseenum = g2.Fuse;
                _hiSet = g2.HiSetMul;
                _maxTripTime = g2.MaxTripTime;
                _minTripMultiplier = g2.MinTripMultiplier;
            }
        }

        public override double Curve(double d)
        {
            double p = Pickup*2;
            if (d >= p*_hiSet)
                return _minTripHardLimit;
            if (d < p * _minTripMultiplier)
                return _maxTripTimeHardLimit;
            double tripTime = A / (Math.Pow(d / p, 2) - 1);
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
