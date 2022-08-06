using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

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

    public partial class TripSaver : FixedMarginCharacteristic
    {
        [ObservableProperty]
        private TripSaverFuse fuseEnum;

        [ObservableProperty]
        private double hiSetMul = double.PositiveInfinity;

        [ObservableProperty]
        private double maxTripTime = 2.0;

        [ObservableProperty]
        private double minTripMultiplier = 3.0;

        private double _maxTripTimeHardLimit = 1e6;
        private double _minTripHardLimit = 0.01;

        
        public double Pickup
        {
            get
            {
                string r = FuseEnum.ToString().TrimEnd(new char[] { 'T', 'K' }).TrimStart(new char[] { '_' });
                return double.Parse(r);
            }
        }

        private double A
        {
            get
            {
                switch (FuseEnum) {
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

        protected override void PretendCopyConstructor(ProtectionCharacteristic c)
        {
            base.PretendCopyConstructor(c);
            if (c is TripSaver c2)
            {
                FuseEnum = c2.FuseEnum;
                HiSetMul = c2.HiSetMul;
                MaxTripTime = c2.MaxTripTime;
                MinTripMultiplier = c2.MinTripMultiplier;
            }
        }

        public override double Curve(double d)
        {
            double p = Pickup*2;
            if (d >= p*HiSetMul)
                return _minTripHardLimit;
            if (d < p * MinTripMultiplier)
                return _maxTripTimeHardLimit;
            double tripTime = A / (Math.Pow(d / p, 2) - 1);
            if (tripTime > MaxTripTime)
                return MaxTripTime;
            if (tripTime > _maxTripTimeHardLimit)
                return _maxTripTimeHardLimit;
            if (tripTime < _minTripHardLimit)
                return _minTripHardLimit;
            return tripTime;
        }
    }
}
