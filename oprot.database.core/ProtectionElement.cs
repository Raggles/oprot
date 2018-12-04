using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.database.core
{
    public class ProtectionElement
    {
        public int ProtectionElementId { get; set; }
        public ProtectionRelay Relay { get; set; }
        public int ProtectionRelayId { get; set; }
        public string ANSIName { get; set; }
        public string Curve { get; set; }
        public double? Pickup { get; set; }
        public double? TMS { get; set; }
        public double? DefT { get; set; }
        public double? DeadTime1 { get; set; }
        public double? DeadTime2 { get; set; }
        public double? DeadTime3 { get; set; }
        public int? TripsToLockout { get; set; }
        public double? ReclaimTime { get; set; }
    }
}
