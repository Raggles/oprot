namespace oprot.plot.core
{
    public class SandCFuseK :FuseDualCharacteristic
    {
        public SandCFuseK(GraphFeature g = null) : base(@".\Curves\SandCMeltingK.txt", @".\Curves\SandCClearingK.txt", g) { }

        public override string ToString()
        {
            return $" (S&C {FuseSize})";
        }
    }

    public class SandCFuseT : FuseDualCharacteristic
    {
        public SandCFuseT(GraphFeature g = null) : base(@".\Curves\SandCMeltingT.txt", @".\Curves\SandCClearingT.txt", g) { }

        public override string ToString()
        {
            return $" (S&C {FuseSize})";
        }
    }
}
