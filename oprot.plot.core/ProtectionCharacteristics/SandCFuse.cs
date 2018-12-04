namespace oprot.plot.core
{
    public class SandCFuseK :Fuse
    {
        public SandCFuseK(GraphFeature g = null) : base(@".\Curves\SandCMeltingK.txt", @".\Curves\SandCClearingK.txt", g) { }
    }

    public class SandCFuseT : Fuse
    {
        public SandCFuseT(GraphFeature g = null) : base(@".\Curves\SandCMeltingT.txt", @".\Curves\SandCClearingT.txt", g) { }
    }
}
