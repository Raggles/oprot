using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class HRCBoltedFuse : FuseSingleCharacteristic
    {
        public HRCBoltedFuse() : base(@".\Curves\HRC Bolted.txt") { }

        public override string ToString()
        {
            return $"Bussmann gG {FuseSize}";
        }
    }
}
