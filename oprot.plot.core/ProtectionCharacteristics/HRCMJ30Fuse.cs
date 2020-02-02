using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class HRCMJ30Fuse : FuseSingleCharacteristic
    {
        public HRCMJ30Fuse(GraphFeature g = null) : base(@".\Curves\HRC MJ30.txt", g) { }

        public override string ToString()
        {
            return $" (Bussmann gU {FuseSize})";
        }
    }
}
