using System;

namespace oprot.plot.core
{
    public class IECStandardInverse : GenericInverseCharacteristic
    {
        protected override double CurveEquation(double d)
        {
            return Tms * 0.14 / (Math.Pow(d / Pickup, 0.02) - 1);
        }

        public override string ToString() =>$"{Pickup}A@{Tms} IEC SI";
    }

    public class IECVeryInverse : GenericInverseCharacteristic
    {
        protected override double CurveEquation(double d)
        {
            return Tms * 13.5 / ((d / Pickup) - 1);
        }

        public override string ToString() => $"{Pickup}A@{Tms} IEC VI";
    }

    public class IECExtremelyInverse : GenericInverseCharacteristic
    {
        protected override double CurveEquation(double d)
        {
            return Tms * 80 / (Math.Pow(d / Pickup, 2.0) - 1);
        }

        public override string ToString() => $"{Pickup}A@{Tms} IEC EI";

    }

    public class IECLongTimeStandardEarthFault : GenericInverseCharacteristic
    {
        protected override double CurveEquation(double d)
        {
            return Tms * 120 / ((d / Pickup) - 1);
        }

        public override string ToString() => $"{Pickup}A@{Tms} IEC LTSEF";
    }
}
