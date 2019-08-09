namespace oprot.plot.core
{
    class ChanceFuseK : FuseDualCharacteristic
    {
        public ChanceFuseK(GraphFeature g = null) : base(@".\Curves\ChanceMeltingK.txt", @".\Curves\ChanceClearingK.txt", g) { }

        public override string ToString()
        {
            return $" (Chance {FuseSize})";
        }
    }

    class ChanceFuseT : FuseDualCharacteristic
    {
        public ChanceFuseT(GraphFeature g = null) : base(@".\Curves\ChanceMeltingT.txt", @".\Curves\ChanceClearingT.txt", g) { }

        public override string ToString()
        {
            return $" (Chance {FuseSize})";
        }
    }

}
