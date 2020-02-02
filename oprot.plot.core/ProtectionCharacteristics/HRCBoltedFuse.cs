using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class HRCBoltedFuse : FuseSingleCharacteristic
    {
        public HRCBoltedFuse(GraphFeature g = null) : base(@".\Curves\HRC Bolted.txt", g) { }

        public override string ToString()
        {
            return $" (Bussmann gG {FuseSize})";
        }
    }
}
