using System;
using System.Collections.Generic;
using OxyPlot;

namespace oprot.plot.core
{
    public enum FuseCurveType
    {
        MinimumMeltingTime,
        MaximumClearingTime
    }

    public class FuseDualCharacteristic : ProtectionCharacteristic
    {
        public List<string> FuseSizes { get; set; } = new List<string>();
        //private FuseCurveType _fusecurve;
        //possibly an ugly way of achieving this, but it works for now
        private Dictionary<string, Dictionary<FuseCurveType, DataInterpolator>> _mapping = new Dictionary<string, Dictionary<FuseCurveType, DataInterpolator>>();
        //private string _fuseSize;

        public FuseCurveType FuseCurve { get; set; }

        public string FuseSize { get; set; }

        public FuseDualCharacteristic(string meltingFile, string clearingFile)
        {
            var melting = CustomCurveParser.ParseFile(meltingFile);
            var clearing = CustomCurveParser.ParseFile(clearingFile);

            //TODO: check that all the same keys are present in melting and clearing
            foreach (var kvp in melting)
            {
                _mapping.Add(kvp.Key, new Dictionary<FuseCurveType, DataInterpolator>() { { FuseCurveType.MinimumMeltingTime, new DataInterpolator(kvp.Value) }, { FuseCurveType.MaximumClearingTime, new DataInterpolator(clearing[kvp.Key]) } });
                FuseSizes.Add(kvp.Key);
            }
            FuseSize = FuseSizes[0];
        }

        public Func<double, double> GetCurve(string r, FuseCurveType t)
        {
            Func<double, double> func = (x) =>
            {
                return _mapping[r][t].Interpolate(x);
            };
            return func;
        }

        public override double Curve(double d)
        {
            return GetCurve(FuseSize, FuseCurve)(d);
        }

        public override string ToString()
        {
            return $"{FuseSize}";
        }

        public override double LowerMargin(double d)
        {
            throw new NotImplementedException();
        }

        public override double UpperMargin(double d)
        {
            throw new NotImplementedException();
        }

        
        protected override PlotElement GetPlotElement()
        {
            //TODO: this is backwards, we can rewrite this to give the correct info now
            var s = new FuseSeries(this, PlotParameters.MinimumCurrent, PlotParameters.MaximumCurrent, PlotParameters.NumberOfSamples, DisplayName, TempMultiplier* PlotParameters.BaseVoltage/ Voltage);
            s.Color = this.Color;
            if (TempMultiplier != 1.0)
                s.LineStyle = LineStyle.Dash;
            return s;
        }
    }
}
