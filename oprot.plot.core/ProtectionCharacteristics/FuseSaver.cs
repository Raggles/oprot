using System;
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

    public class FuseSaver : ProtectionCharacteristic
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
            get { return _fuseobject; }
            set
            {
                _fuseobject = value;
                _fuseobject.PropertyChanged += _fuseobject_PropertyChanged;
                RaisePropertyChanged();
                UpdateGraphElement();
            }
        }

        public double MaxTripTime
        {
            get
            {
                return _maxTripTime;
            }
            set
            {
                _maxTripTime = value;
                RaisePropertyChanged(nameof(MaxTripTime));
                UpdateGraphElement();
            }
        }

        public double HiSetMul
        {
            get
            {
                return _hiSet;
            }
            set
            {
                _hiSet = value;
                RaisePropertyChanged();
                UpdateGraphElement();
            }
        }

        public double MinTripMultiplier
        {
            get
            {
                return _minTripMultiplier;
            }
            set
            {
                _minTripMultiplier = value;
                RaisePropertyChanged();
                UpdateGraphElement();
            }
        }

        private void _fuseobject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Fuse));
            RaisePropertyChanged(nameof(Description));
            UpdateGraphElement();
        }

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
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Description));
                //Fuse will update the curve
            }
        }

        public double Pickup
        {
            get
            {
                string r = _fuseobject.FuseSize.TrimEnd(new char[] { 'T', 'K' });
                return double.Parse(r);
            }
        }

        public override OxyColor Color
        {
            get
            {
                return _plotElement == null ? _color : ((LogFunctionSeries)_plotElement).ActualColor;
            }
            set
            {
                _color = value;
                if (_plotElement != null)
                    ((LogFunctionSeries)_plotElement).Color = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DisplayColor));
                RaiseGraphElementInvalidated();
            }
        }

        public FuseSaver(GraphFeature g = null) : base(g)
        {
            if (g is FuseSaver g2)
            {
                _fuseenum = g2.FuseType;
                _fuseobject = g2.Fuse.Clone() as FuseDualCharacteristic;
                
                _hiSet = g2.HiSetMul;
                _maxTripTime = g2.MaxTripTime;
                _minTripMultiplier = g2.MinTripMultiplier;
            }
            if (_fuseobject == null)
                    FuseType = _fuseenum;
        }

        public override double Curve(double d)
        {
            if (!_i2tDict.ContainsKey(_fuseobject.FuseSize)){
                return double.NaN;
            }
            double p = Pickup;
            if (d >= p*_hiSet)
                return 0.01;
            if (d < p * _minTripMultiplier)
                return _maxTripTimeHardLimit;
            double tripTime = 0.33 * _i2tDict[_fuseobject.FuseSize] / Math.Pow(d,2);
            if (tripTime > _maxTripTime)
                return _maxTripTime;
            if (tripTime > _maxTripTimeHardLimit)
                return _maxTripTimeHardLimit;
            if (tripTime < _minTripHardLimit)
                return _minTripHardLimit;
            return tripTime;
        }

        public override PlotElement GetPlotElement()
        {
            var s = new LogFunctionSeries(Curve, _minimumCurrent, _maximumCurrent, _numberSamples, DisplayName, DiscriminationMargin, _tempMultiplier * _baseVoltage / _voltage);
            s.ShowDiscriminationMargin = ShowDiscriminationMargin;
            s.Color = _color;
            if (_tempMultiplier != 1.0)
                s.LineStyle = LineStyle.Dash;
            return s;
        }

        private bool _showDiscriminationMargin = true;
        private double _discriminationMargin = 0.2;

        public bool ShowDiscriminationMargin
        {
            get
            {
                return _showDiscriminationMargin;
            }
            set
            {
                _showDiscriminationMargin = value;
                ((LogFunctionSeries)_plotElement).ShowDiscriminationMargin = value;
                RaiseGraphElementInvalidated();
                RaisePropertyChanged(nameof(ShowDiscriminationMargin));
            }
        }

        public double DiscriminationMargin
        {
            get
            {
                return _discriminationMargin;
            }
            set
            {
                if (value < 0.01 || value > 1)
                    return;
                _discriminationMargin = value;
                RaisePropertyChanged(nameof(DiscriminationMargin));
                UpdateGraphElement();
            }
        }

        public override string ToString()
        {
            return Fuse.ToString();
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
