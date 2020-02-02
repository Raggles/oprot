using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class HRCKnifeFuse : FuseSingleCharacteristic
    {
        public HRCKnifeFuse(GraphFeature g = null) : base(@".\Curves\HRC Knife.txt", g) { }

        public override string ToString()
        {
            return $" (Bussmann gG {FuseSize})";
        }
    }
}
