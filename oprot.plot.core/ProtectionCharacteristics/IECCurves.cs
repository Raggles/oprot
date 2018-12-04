using System;

namespace oprot.plot.core
{
    public class IECStandardInverse : GenericInverseCharacteristic
    {
        public IECStandardInverse() : base() { }
        public IECStandardInverse(GraphFeature g) : base(g) { }

        protected override double CurveEquation(double d)
        {
            return _tms * 0.14 / (Math.Pow(d / _pickup, 0.02) - 1);
        }

        public override string ToString()
        {
            return $" ({Pickup}A@{TMS} IEC SI)";
        }
    }

    public class IECVeryInverse : GenericInverseCharacteristic
    {
        public IECVeryInverse() : base() { }
        public IECVeryInverse(GraphFeature g) : base(g) { }

        protected override double CurveEquation(double d)
        {
            return _tms * 13.5 / ((d / _pickup) - 1);
        }

        public override string ToString()
        {
            return $" ({Pickup}A@{TMS} IEC VI)";
        }
    }

    public class IECExtremelyInverse : GenericInverseCharacteristic
    {
        public IECExtremelyInverse() : base() { }
        public IECExtremelyInverse(GraphFeature g) : base(g) { }

        protected override double CurveEquation(double d)
        {
            return _tms * 80 / (Math.Pow(d / _pickup, 2.0) - 1);
        }

        public override string ToString()
        {
            return $" ({Pickup}A@{TMS} IEC EI)";
        }

    }

    public class IECLongTimeStandardEarthFault : GenericInverseCharacteristic
    {
        public IECLongTimeStandardEarthFault() : base() { }
        public IECLongTimeStandardEarthFault(GraphFeature g) : base(g) { }

        protected override double CurveEquation(double d)
        {
            return _tms * 120 / ((d / _pickup) - 1);
        }

        public override string ToString()
        {
            return $" ({Pickup}A@{TMS} IEC LTSEF)";
        }
    }
}
