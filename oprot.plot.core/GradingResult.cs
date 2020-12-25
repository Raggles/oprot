using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class GradingResult
    {
        public ProtectionCharacteristic Curve1 { get; }
        public ProtectionCharacteristic Cirve2 { get; }
        public List<Point> NonGradingSections { get; }
        public override string ToString()
        {
            return "";
        }
    }
}
