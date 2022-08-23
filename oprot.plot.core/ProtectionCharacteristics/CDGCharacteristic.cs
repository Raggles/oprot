using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace oprot.plot.core
{
    public partial class CDGCharacteristic : FixedMarginCharacteristic
    {
        private Dictionary<double, Point[]> _points = new Dictionary<double, Point[]>();
        private Dictionary<double, DataInterpolator> _data = new Dictionary<double, DataInterpolator>();

        [ObservableProperty]
        private double plugSetting;

        [ObservableProperty]
        private double timeSetting = 1.0;
        //throw new ArgumentOutOfRangeException("Time setting must be between 0.1 and 1.0");

        [ObservableProperty]
        private double cTRatio;

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
            double pickup = d / CTRatio;

            if (_data.ContainsKey(TimeSetting))
            {
                var r = _data[TimeSetting].Interpolate(pickup);
                return r;
            }
            else
            {
                double tmsLo = RoundDown(TimeSetting, 1);
                double tmsHi = RoundUp(TimeSetting, 1);
                double valueLo = _data[tmsLo].Interpolate(pickup);
                double valueHi = _data[tmsHi].Interpolate(pickup);

                double ratio = (TimeSetting - tmsLo) / (tmsHi - tmsLo);
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
    }
}
