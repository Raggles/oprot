using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace oprot.plot.core
{

    public partial class FuseSingleCharacteristic : FixedMarginCharacteristic
    {
        public List<string> FuseSizes { get; set; } = new List<string>();
        private Dictionary<string, DataInterpolator> _mapping = new Dictionary<string, DataInterpolator>();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Description))]
        private string fuseSize;
        
        
        public FuseSingleCharacteristic(string dataFile) 
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

        public override string ToString() => $"{FuseSize}";

    }
}
