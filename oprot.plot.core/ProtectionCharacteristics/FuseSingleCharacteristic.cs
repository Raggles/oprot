using System;
using System.Collections.Generic;
using OxyPlot;

namespace oprot.plot.core
{

    public class FuseSingleCharacteristic : ProtectionCharacteristic
    {
        public List<string> FuseSizes { get; set; } = new List<string>();
        private Dictionary<string, DataInterpolator> _mapping = new Dictionary<string, DataInterpolator>();
        private string _fuseSize;

        public string FuseSize
        {
            get
            {
                return _fuseSize;
            }
            set
            {
                _fuseSize = value;
                RaisePropertyChanged(nameof(FuseSize));
                RaisePropertyChanged(nameof(Description));
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

        public FuseSingleCharacteristic(string dataFile, GraphFeature g = null) : base(g)
        {
            var data = CustomCurveParser.ParseFile(dataFile);

            foreach (var kvp in data)
            {
                _mapping.Add(kvp.Key, new DataInterpolator(kvp.Value));
                FuseSizes.Add(kvp.Key);
            }
            FuseSize = FuseSizes[0];
        }

        public Func<double, double> GetCurve(string r)
        {
            double func(double x)
            {
                return _mapping[r].Interpolate(x);
            }
            return func;
        }

        public override double Curve(double d)
        {
            return GetCurve(FuseSize)(d);
        }

        public override string ToString()
        {
            return $" ({FuseSize})";
        }

        public override PlotElement GetPlotElement()
        {
            //TODO: this is backwards, we can rewrite this to give the correct info now
            var s = new LogFunctionSeries(Curve, _minimumCurrent, _maximumCurrent, _numberSamples, DisplayName, 0.02, _tempMultiplier * _baseVoltage / _voltage);
            s.Color = _color;
            if (_tempMultiplier != 1.0)
                s.LineStyle = LineStyle.Dash;
            return s;
        }
    }
}
