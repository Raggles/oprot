namespace oprot.plot.core
{
    class ChanceFuseK : Fuse
    {
        public ChanceFuseK(GraphFeature g = null) : base(@".\Curves\ChanceMeltingK.txt", @".\Curves\ChanceClearingK.txt", g) { }
    }

    class ChanceFuseT : Fuse
    {
        public ChanceFuseT(GraphFeature g = null) : base(@".\Curves\ChanceMeltingT.txt", @".\Curves\ChanceClearingT.txt", g) { }
    }

}
