using MicroMvvm;
using System;
using System.Collections.Generic;

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
        [Obsolete]
        public abstract double Curve(double d);

        //TODO: I've forgotten what this code was intended for???  maybe melting and clearing?? or fusesaver & fuse
        public List<string> CurveNames { get; }
        public double Curve(double d, int index) { return double.NaN; }
        public double Curve(double d, string name) { return double.NaN; }

        public override string ToString()
        {
            return "";
        }

        //public abstract double MaximumMargin(double d);
        //public abstract double MinimumMargin(double d);
    }
}
