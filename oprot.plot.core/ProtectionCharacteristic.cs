using MicroMvvm;
using System;

namespace oprot.plot.core
{
    public abstract class ProtectionCharacteristic: GraphFeature
    {
        public ProtectionCharacteristic() : base() { }
        public ProtectionCharacteristic(GraphFeature g) : base(g) { }

        /// <summary>
        /// The equation for the curve as a function of current
        /// </summary>
        /// <param name="d">Current</param>
        /// <returns></returns>
        public abstract double Curve(double d);

        public override string ToString()
        {
            return "";
        }

        //public abstract double MaximumMargin(double d);
        //public abstract double MinimumMargin(double d);
    }
}
