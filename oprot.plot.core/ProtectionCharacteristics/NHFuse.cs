using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class NHFuse : FuseSingleCharacteristic
    {
        public NHFuse() : base(@".\Curves\Bussmann NH LV Fuse 690V.txt") { }

        public override string ToString()
        {
            return $"NH gG/gL {FuseSize}";
        }
    }
}
