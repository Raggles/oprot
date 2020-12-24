using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class HRCKnifeFuse : FuseSingleCharacteristic
    {
        public HRCKnifeFuse() : base(@".\Curves\HRC Knife.txt") { }

        public override string ToString()
        {
            return $"Bussmann gG {FuseSize}";
        }
    }
}
