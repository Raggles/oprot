﻿using System;
using System.Collections.Generic;
using System.Text;
using OxyPlot;

namespace oprot.plot.core
{
    public enum FuseSaverFuse
    {
        SandCPositrolFuseTypeK,
        SandCPositrolFuseTypeT,
        ChanceFuseTypeK,
        ChanceFuseTypeT
    }

    public class FuseSaver : FixedMarginCharacteristic
    {
        private FuseDualCharacteristic _fuseobject;
        private FuseSaverFuse _fuseenum = FuseSaverFuse.SandCPositrolFuseTypeT;
        private double _hiSet = double.PositiveInfinity;
        private double _maxTripTime = 2.0;
        private double _minTripMultiplier = 3.0;
        private double _maxTripTimeHardLimit = 1e6;
        private double _minTripHardLimit = 0.01;

        public FuseDualCharacteristic Fuse
        {
            get
            {
                return _fuseobject;
            }
            set
            {
                _fuseobject = value;
                _fuseobject.FeatureChanged += _fuseobject_FeatureChanged;
                _fuseobject.PropertyChanged += _fuseobject_PropertyChanged;
            }
        }

        private void _fuseobject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Description))
            {
                RaisePropertyChanged(e.PropertyName);
                RaisePropertyChanged(nameof(DisplayName));
            }
        }

        private void _fuseobject_FeatureChanged()
        {
            _plotElement = null;
            RaiseFeatureChanged();
        }

        public double MaxTripTime { get; set; }
      

        public double HiSetMul { get; set; }
       

        public double MinTripMultiplier { get; set; }


        public FuseSaverFuse FuseType {
            get { return _fuseenum; }
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
                _fuseenum = value;
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

        protected override void PretendCopyConstructor(GraphableFeature g)
        {
            base.PretendCopyConstructor(g);
            if (g is FuseSaver g2)
            {
                _fuseenum = g2.FuseType;
                Fuse = g2.Fuse.Clone() as FuseDualCharacteristic;
                
                _hiSet = g2.HiSetMul;
                _maxTripTime = g2.MaxTripTime;
                _minTripMultiplier = g2.MinTripMultiplier;
            }
            if (Fuse == null)
                    FuseType = _fuseenum;
        }

        public override double Curve(double d)
        {
            if (!_i2tDict.ContainsKey(Fuse.FuseSize)){
                return double.NaN;
            }
            double p = Pickup;
            if (d >= p*_hiSet)
                return 0.01;
            if (d < p * _minTripMultiplier)
                return _maxTripTimeHardLimit;
            double tripTime = 0.33 * _i2tDict[Fuse.FuseSize] / Math.Pow(d,2);
            if (tripTime > _maxTripTime)
                return _maxTripTime;
            if (tripTime > _maxTripTimeHardLimit)
                return _maxTripTimeHardLimit;
            if (tripTime < _minTripHardLimit)
                return _minTripHardLimit;
            return tripTime;
        }

        
        public override string ToString()
        {
            return Fuse?.ToString();
        }

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
