using System;
using System.Collections.Generic;
using System.Windows;
using OxyPlot;

namespace oprot.plot.core
{
    public class CDGCharacteristic : ProtectionCharacteristic
    {
        private double _plugSetting = 1.0;
        private double _timeSetting = 1.0;
        private double _ctRatio = 400;

        private Dictionary<double, Point[]> _points = new Dictionary<double, Point[]>();
        private Dictionary<double, DataInterpolator> _data = new Dictionary<double, DataInterpolator>();

        public double PlugSetting
        {
            get
            {
                return _plugSetting;
            }
            set
            {
                _plugSetting = value;
                RaisePropertyChanged(nameof(PlugSetting));
            }
        }

        public double TimeSetting
        {
            get
            {
                return _timeSetting;
            }
            set
            {
                if (value < 0.1 || value > 1.0)
                    throw new ArgumentOutOfRangeException("Time setting must be between 0.1 and 1.0");
                _timeSetting = value;
                RaisePropertyChanged(nameof(TimeSetting));
            }
        }

        public double CTRatio
        {
            get
            {
                return _ctRatio;
            }
            set
            {
                _ctRatio = value;
                RaisePropertyChanged(nameof(CTRatio));
            }
        }

        public override OxyColor Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public CDGCharacteristic(string filename)
        {
            var data = CustomCurveParser.ParseFile(filename);
            foreach (var item in data)
            {
                _data.Add(double.Parse(item.Key), new DataInterpolator(item.Value));
            }
        }

        public CDGCharacteristic() : this(@".\Curves\CDG_3s_SI.txt")
        {

        }

        public override double Curve(double d)
        {
            double pickup = d / _ctRatio;

            if (_data.ContainsKey(_timeSetting))
            {
                var r = _data[_timeSetting].Interpolate(pickup);
                return r;
            }
            else
            {
                double tmsLo = RoundDown(_timeSetting, 1);
                double tmsHi = RoundUp(_timeSetting, 1);
                double valueLo = _data[tmsLo].Interpolate(pickup);
                double valueHi = _data[tmsHi].Interpolate(pickup);

                double ratio = (_timeSetting - tmsLo) / (tmsHi - tmsLo);
                return (valueHi - valueLo) * ratio + valueLo;
            }
        }

        private double RoundDown(double i, int decimalPlaces)
        {
            var power = Math.Pow(10, decimalPlaces);
            return Math.Floor(i * power) / power;
        }
        private double RoundUp(double i, int decimalPlaces)
        {
            var power = Math.Pow(10, decimalPlaces);
            return Math.Ceiling(i * power) / power;
        }

        public override PlotElement GetPlotElement()
        {
            throw new NotImplementedException();
        }
    }
}
