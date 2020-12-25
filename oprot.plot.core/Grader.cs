using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class GradingOptions
    {
        public double MinCurrent { get; set; } = 1;
        public double MaxCurrent { get; set; } = 10000;
        public int Samples { get; set; } = 1000;

    }
    public static class Grader
    {
        public static GradingResult Grade(GradingOptions o, ProtectionCharacteristic fastCurve, ProtectionCharacteristic slowCurve)
        {
            return null;
        }

        public static List<GradingResult> Grade (GradingOptions o, List<ProtectionCharacteristic> curves)
        {
            return null;
        }
    }
}
