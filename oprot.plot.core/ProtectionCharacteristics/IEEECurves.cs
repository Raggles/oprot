using System;

namespace oprot.plot.core
{
    public class IEEEModeratelyInverse : GenericInverseCharacteristic
    {
        protected override double CurveEquation(double d)
        {
            return Tms / 7 * (0.0515 / (Math.Pow(d / Pickup, 0.02) - 1) + 0.114);
        }

        public override string ToString() => $"{Pickup}A@{Tms} IEEE MI";
    }

    public class IEEEVeryInverse : GenericInverseCharacteristic
    {
        protected override double CurveEquation(double d)
        {
            return Tms / 7 * (19.61 / (Math.Pow(d / Pickup, 2) - 1) + 0.491);
        }

        public override string ToString() => $"{Pickup}A@{Tms} IEEE VI";
    }

    public class IEEEExtremelyInverse : GenericInverseCharacteristic
    {
        protected override double CurveEquation(double d)
        {
            return Tms / 7 * (28.2 / (Math.Pow(d / Pickup, 2) - 1) + 0.1217);
        }

        public override string ToString() => $"{Pickup}A@{Tms} IEEE EI";
    }

    public class USC08Inverse : GenericInverseCharacteristic
    {
        protected override double CurveEquation(double d)
        {
            return Tms / 7 * (5.95 / (Math.Pow(d / Pickup, 2) - 1) + 0.18);
        }

        public override string ToString() => $"{Pickup}A@{Tms} USC08";
    }

    public class USC02ShortTimeInverse : GenericInverseCharacteristic
    {
        protected override double CurveEquation(double d)
        {
            return Tms / 7 * (0.02394 / (Math.Pow(d / Pickup, 0.02) - 1) + 0.01694);
        }

        public override string ToString() =>$"{Pickup}A@{Tms} USC02";
    }
}