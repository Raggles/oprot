using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace oprot.plot.core
{
    public abstract partial class FixedMarginCharacteristic : ProtectionCharacteristic
    {
        [ObservableProperty]
        private double discriminationMargin = 0.2;

        public override double LowerMargin(double d)
        {
            return Math.Max(Curve(d) - DiscriminationMargin, 0);
        }

        public override double UpperMargin(double d)
        {
            return Curve(d) + DiscriminationMargin;
        }

    }

}
