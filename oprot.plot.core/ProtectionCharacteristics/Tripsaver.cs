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

    public class TripSaver : FixedMarginCharacteristic
    {
        private TripSaverFuse _fuseenum;
        private double _hiSet = double.PositiveInfinity;
        private double _maxTripTime = 2.0;
        private double _minTripMultiplier = 3.0;
        private double _maxTripTimeHardLimit = 1e6;
        private double _minTripHardLimit = 0.01;

        public TripSaverFuse Fuse { get; set; }

        public double MaxTripTime { get; set; }

        public double HiSetMul { get; set; }

        public double MinTripMultiplier { get; set; }
        
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

        protected override void PretendCopyConstructor(GraphableFeature g)
        {
            base.PretendCopyConstructor(g);
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
    }
}
