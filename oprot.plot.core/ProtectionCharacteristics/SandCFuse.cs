namespace oprot.plot.core
{
    public class SandCFuseK :FuseDualCharacteristic
    {
        public SandCFuseK() : base(@".\Curves\SandCMeltingK.txt", @".\Curves\SandCClearingK.txt") { }

        public override string ToString()
        {
            return $"S&C {FuseSize}";
        }
    }

    public class SandCFuseT : FuseDualCharacteristic
    {
        public SandCFuseT() : base(@".\Curves\SandCMeltingT.txt", @".\Curves\SandCClearingT.txt") { }

        public override string ToString()
        {
            return $"S&C {FuseSize}";
        }
    }
}
