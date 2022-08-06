using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace oprot.plot.core
{
    public enum FuseSaverFuse
    {
        SandCPositrolFuseTypeK,
        SandCPositrolFuseTypeT,
        ChanceFuseTypeK,
        ChanceFuseTypeT
    }

    public partial class FuseSaver : FixedMarginCharacteristic
    {
        [ObservableProperty]
        private FuseDualCharacteristic fuse;

        [ObservableProperty]
        private FuseSaverFuse fuseEnum = FuseSaverFuse.SandCPositrolFuseTypeT;

        [ObservableProperty]
        private double hiSet = double.PositiveInfinity;

        [ObservableProperty]
        private double maxTripTime = 2.0;

        [ObservableProperty]
        private double minTripMultiplier = 3.0;


        private double _maxTripTimeHardLimit = 1e6;
        private double _minTripHardLimit = 0.01;

        
        public FuseSaverFuse FuseType {
            get { return FuseEnum; }
            set
            {
                switch (value)
                {
                    case FuseSaverFuse.SandCPositrolFuseTypeK:
                        Fuse = new SandCFuseK();
                        break;
                    case FuseSaverFuse.SandCPositrolFuseTypeT:
                        Fuse = new SandCFuseT();
                        break;
                    case FuseSaverFuse.ChanceFuseTypeK:
                        Fuse = new ChanceFuseK();
                        break;
                    case FuseSaverFuse.ChanceFuseTypeT:
                        Fuse = new ChanceFuseT();
                        break;
                }
                FuseEnum = value;
            }
        }

        public double Pickup
        {
            get
            {
                string r = Fuse.FuseSize.TrimEnd(new char[] { 'T', 'K' });
                return double.Parse(r);
            }
        }

        public FuseSaver()
        {
            Fuse = new SandCFuseT();
        }

        protected override void PretendCopyConstructor(ProtectionCharacteristic c)
        {
            base.PretendCopyConstructor(c);
            if (c is FuseSaver c2)
            {
                FuseEnum = c2.FuseType;
                Fuse = c2.Fuse.Clone() as FuseDualCharacteristic;
                HiSet = c2.HiSet;
                MaxTripTime = c2.MaxTripTime;
                MinTripMultiplier = c2.MinTripMultiplier;
            }
            if (Fuse == null)
                    FuseType = FuseEnum;
        }

        public override double Curve(double d)
        {
            if (!_i2tDict.ContainsKey(Fuse.FuseSize)){
                return double.NaN;
            }
            double p = Pickup;
            if (d >= p*HiSet)
                return 0.01;
            if (d < p * MinTripMultiplier)
                return _maxTripTimeHardLimit;
            double tripTime = 0.33 * _i2tDict[Fuse.FuseSize] / Math.Pow(d,2);
            if (tripTime > MaxTripTime)
                return MaxTripTime;
            if (tripTime > _maxTripTimeHardLimit)
                return _maxTripTimeHardLimit;
            if (tripTime < _minTripHardLimit)
                return _minTripHardLimit;
            return tripTime;
        }

        
        public override string ToString() =>Fuse?.ToString();

        private Dictionary<string, double> _i2tDict = new Dictionary<string, double>() {
            {"1K",19        }  ,
            {"2K",91        }  ,
            {"3K",210       }  ,
            {"6K",500       }  ,
            {"8K",900       }  ,
            {"10K",1600     }  ,
            {"12K",2800     }  ,
            {"15K",4900     }  ,
            {"20K",7200     }  ,
            {"25K",13500    }  ,
            {"30K",21000    }  ,
            {"40K",32000    }  ,
            {"50K",51000    }  ,
            {"65K",86000    }  ,
            {"80K",135000   }  ,
            {"100K",250000  }  ,
            {"1T",20        }  ,
            {"2T",92        }  ,
            {"3T",220       }  ,
            {"6T",1500      }  ,
            {"8T",2900      }  ,
            {"10T",5000     }  ,
            {"12T",9000     }  ,
            {"15T",15000    }  ,
            {"20T",26000    }  ,
            {"25T",38000    }  ,
            {"30T",68000    }  ,
            {"40T",120000   }  ,
            {"50T",180000   }  ,
            {"65T",270000   }  ,
            {"80T",450000   }  ,
            {"100T",680000  }
        };

    }
}
