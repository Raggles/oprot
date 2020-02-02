using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class ABBCEFFuse : FuseSingleCharacteristic
    {
        public ABBCEFFuse(GraphFeature g = null) : base(@".\Curves\ABB CEF.txt", g) { }

        public override string ToString()
        {
            return $" (ABB {FuseSize})";
        }
    }
}
