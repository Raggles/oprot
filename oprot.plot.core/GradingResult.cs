using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class GradingResult
    {
        public ProtectionCharacteristic Curve1 { get; init; }
        public ProtectionCharacteristic Curve2 { get; set; }
        public List<GradingSection> Sections { get; set; } = new List<GradingSection>();
        public bool Grades => Sections.Count == 1;
        public override string ToString()
        {
            string g = Grades ? "grade" : "don't grade";
            string r = $"[{Curve1.Name}] (fast) and [{Curve2.Name}] (slow) {g}:";
            foreach (var s in Sections)
            {
                r += Environment.NewLine + "  -" + s.ToString();                    
            }
            return r;
        }
    }

    public struct GradingSection
    {
        public double From;
        public double To;
        public bool Grades;

        public override string ToString()
        {
            var g = Grades ? "Grades" : "Doesn't grade";
            return $"{g} from {From}A to {To}A";
        }
    }
}
