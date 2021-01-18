namespace oprot.plot.core
{
    public class ChanceFuseK : FuseDualCharacteristic
    {
        public ChanceFuseK() : base(@".\Curves\ChanceMeltingK.txt", @".\Curves\ChanceClearingK.txt") { }

        public override string ToString()
        {
            return $"Chance {FuseSize}";
        }
    }

    public class ChanceFuseT : FuseDualCharacteristic
    {
        public ChanceFuseT() : base(@".\Curves\ChanceMeltingT.txt", @".\Curves\ChanceClearingT.txt") { }

        public override string ToString()
        {
            return $"Chance {FuseSize}";
        }
    }

}
