using CommunityToolkit.Mvvm.ComponentModel;

namespace oprot.plot.core
{
    public partial class DefiniteTimeCharacteristic : FixedMarginCharacteristic
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Description))]
        private double time = 10;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Description))]
        private double pickup = 100;

        [ObservableProperty]
        private double hiSet = double.PositiveInfinity;


        private readonly double _maxTripTimeHardLimit = 1e6;

        protected override void PretendCopyConstructor(ProtectionCharacteristic c)
        {
            base.PretendCopyConstructor(c);
            if (c is DefiniteTimeCharacteristic c2)
            {
                Time = c2.Time;
                Pickup = c2.Pickup;
                HiSet = c2.HiSet;
            }
        }
        
        public override double Curve(double d)
        {
            if (d >= HiSet)
                return 0.01;
            if (d >= Pickup)
            {
                return Time;
            }
            else
                return _maxTripTimeHardLimit;
        }

        public override string ToString() => $"{Pickup}A @{Time} DT";
        

    }
}
