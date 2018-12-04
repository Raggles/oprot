using System;

namespace oprot.plot.core
{
    public class IEEEModeratelyInverse : GenericInverseCharacteristic
    {
        public IEEEModeratelyInverse() : base() { }
        public IEEEModeratelyInverse(GraphFeature g) : base(g) { }
        public IEEEModeratelyInverse(GenericInverseCharacteristic g) : base(g) { }

        protected override double CurveEquation(double d)
        {
            return _tms / 7 * (0.0515 / (Math.Pow(d / _pickup, 0.02) - 1) + 0.114);
        }

        public override string ToString()
        {
            return $" ({Pickup}A@{TMS} IEEE MI)";
        }
    }

    public class IEEEVeryInverse : GenericInverseCharacteristic
    {
        public IEEEVeryInverse() : base() { }
        public IEEEVeryInverse(GraphFeature g) : base(g) { }
        public IEEEVeryInverse(GenericInverseCharacteristic g) : base(g) { }

        protected override double CurveEquation(double d)
        {
            return _tms / 7 * (19.61 / (Math.Pow(d / _pickup, 2) - 1) + 0.491);
        }

        public override string ToString()
        {
            return $" ({Pickup}A@{TMS} IEEE VI)";
        }
    }

    public class IEEEExtremelyInverse : GenericInverseCharacteristic
    {
        public IEEEExtremelyInverse() : base() { }
        public IEEEExtremelyInverse(GraphFeature g) : base(g) { }
        public IEEEExtremelyInverse(GenericInverseCharacteristic g) : base(g) { }

        protected override double CurveEquation(double d)
        {
            return _tms / 7 * (28.2 / (Math.Pow(d / _pickup, 2) - 1) + 0.1217);
        }

        public override string ToString()
        {
            return $" ({Pickup}A@{TMS} IEEE EI)";
        }
    }

    public class USC08Inverse : GenericInverseCharacteristic
    {
        public USC08Inverse() : base() { }
        public USC08Inverse(GraphFeature g) : base(g) { }
        public USC08Inverse(GenericInverseCharacteristic g) : base(g) { }

        protected override double CurveEquation(double d)
        {
            return _tms / 7 * (5.95 / (Math.Pow(d / _pickup, 2) - 1) + 0.18);
        }

        public override string ToString()
        {
            return $" ({Pickup}A@{TMS} USC08)";
        }
    }

    public class USC02ShortTimeInverse : GenericInverseCharacteristic
    {
        public USC02ShortTimeInverse() : base() { }
        public USC02ShortTimeInverse(GraphFeature g) : base(g) { }
        public USC02ShortTimeInverse(GenericInverseCharacteristic g) : base(g) { }

        protected override double CurveEquation(double d)
        {
            return _tms / 7 * (0.02394 / (Math.Pow(d / _pickup, 0.02) - 1) + 0.01694);
        }

        public override string ToString()
        {
            return $" ({Pickup}A@{TMS} USC02)";
        }
    }
}