using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public class ABBCEFFuse : FuseSingleCharacteristic
    {
        public ABBCEFFuse() : base(@".\Curves\ABB CEF.txt") { }

        public override string ToString()
        {
            return $"ABB {FuseSize}";
        }
    }
}
