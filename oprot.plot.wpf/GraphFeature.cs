using MicroMvvm;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Windows.Input;
using oprot.plot.core;

namespace oprot.plot.wpf
{
    public class GraphFeature : ObservableObject
    {
        //whenever a curve parameter has changed, i.e. the plot element has been regenerated
        public event Action GraphFeatureChanged;
        
        //whenever the plot element has not changed, but required re-rendering.
        //e.g. the discrimination margin has been enabled/disabled
        //curently not used
        public event Action GraphFeatureInvalidated;


        private GraphFeatureKind _featureType = GraphFeatureKind.IECStandardInverse;
        private bool _initialized = false;

        [AlsoNotifyFor(nameof(Feature))]
        public GraphFeatureKind FeatureType
        {
            get
            {
                return _featureType;
            }
            set
            {
                if (value == GraphFeatureKind.GradingResult && _initialized)
                    return;
                
                _featureType = value;

                if (_initialized && value != GraphFeatureKind.GradingResult)
                {
                    SetNewFeature();
                }
                RaiseFeatureChanged();
            }
        }

        [JsonProperty]
        public GraphableFeature Feature { get; private set; }
        

        public GraphFeature() {  }

        public GraphFeature(GraphableFeature f, GraphFeatureKind k)
        {
            _initialized = true;
            _featureType = k;
            Feature = f;
            Feature.FeatureChanged += _curveObject_PropertyChanged;
        }

        public object Clone()
        {
            GraphFeature obj = (GraphFeature)this.MemberwiseClone();
            obj.CloneClean();
            return obj;
        }

        private void CloneClean()
        {
            GraphFeatureChanged = null;
            GraphFeatureInvalidated = null;

            Feature = (GraphableFeature)Feature.Clone();
            Feature.FeatureChanged += _curveObject_PropertyChanged;
        }
        


        private void RaiseFeatureChanged()
        {
            GraphFeatureChanged?.Invoke();
        }

        private void _curveObject_PropertyChanged()
        {
            RaiseFeatureChanged();
        }
        private void SetNewFeature()
        {
            switch (_featureType)
            {
                case GraphFeatureKind.DefiniteTime:
                    Feature = GraphableFeature.FromOther<DefiniteTimeCharacteristic>(Feature);
                    break;
                case GraphFeatureKind.IECStandardInverse:
                    Feature = GraphableFeature.FromOther<IECStandardInverse>(Feature);
                    break;
                case GraphFeatureKind.IECVeryInverse:
                    Feature = GraphableFeature.FromOther<IECVeryInverse>(Feature); 
                    break;
                case GraphFeatureKind.IECExtremelyInverse:
                    Feature = GraphableFeature.FromOther<IECExtremelyInverse>(Feature);
                    break;
                case GraphFeatureKind.IEEEModeratelyInverse:
                    Feature = GraphableFeature.FromOther<IEEEModeratelyInverse>(Feature);
                    break;
                case GraphFeatureKind.IEEEVeryInverse:
                    Feature = GraphableFeature.FromOther<IEEEVeryInverse>(Feature);
                    break;
                case GraphFeatureKind.IEEEExtremelyInverse:
                    Feature = GraphableFeature.FromOther<IEEEExtremelyInverse>(Feature);
                    break;
                case GraphFeatureKind.SandCPositrolFuseTypeK:
                    Feature = GraphableFeature.FromOther<SandCFuseK>(Feature);
                    break;
                case GraphFeatureKind.SandCPositrolFuseTypeT:
                    Feature = GraphableFeature.FromOther<SandCFuseT>(Feature);
                    break;
                case GraphFeatureKind.ChanceFuseTypeK:
                    Feature = GraphableFeature.FromOther<ChanceFuseK>(Feature);
                    break;
                case GraphFeatureKind.ChanceFuseTypeT:
                    Feature = GraphableFeature.FromOther<ChanceFuseT>(Feature);
                    break;
                case GraphFeatureKind.FuseSaver:
                    Feature = GraphableFeature.FromOther<FuseSaver>(Feature);
                    break;
                case GraphFeatureKind.TripSaver:
                    Feature = GraphableFeature.FromOther<TripSaver>(Feature);
                    break;
                case GraphFeatureKind.NHgGFuse690V:
                    Feature = GraphableFeature.FromOther<NHFuse>(Feature);
                    break;
                case GraphFeatureKind.HRCBoltedFuse:
                    Feature = GraphableFeature.FromOther<HRCBoltedFuse>(Feature);
                    break;
                case GraphFeatureKind.HRCKnifeFuse:
                    Feature = GraphableFeature.FromOther<HRCKnifeFuse>(Feature);
                    break;
                case GraphFeatureKind.HRCMJTypeFuse:
                    Feature = GraphableFeature.FromOther<HRCMJ30Fuse>(Feature);
                    break;
                case GraphFeatureKind.ABBCEF:
                    Feature = GraphableFeature.FromOther<ABBCEFFuse>(Feature);
                    break;
                case GraphFeatureKind.FaultLevelAnnotation:
                    Feature = GraphableFeature.FromOther<FaultLevelAnnotation>(Feature);
                    break;
            }
            Feature.FeatureChanged += _curveObject_PropertyChanged;
        }

        public void OnDeserialize()
        {
            Feature.FeatureChanged += _curveObject_PropertyChanged;
            _initialized = true;
        }

        #region SetTempMultiplierCommand

        void SetTempMultiplierExecute(object d)
        {
            Feature.TempMultiplier = double.Parse(d.ToString());
        }

        bool CanSetTempMultiplierExecute(object d)
        {
            return true;
        }

        [JsonIgnore]
        public ICommand SetTempMultiplier { get { return new oprot.plot.core.RelayCommand<object>(SetTempMultiplierExecute, CanSetTempMultiplierExecute); } }

        #endregion


    }
}
