using System;
using System.Windows.Input;
using MicroMvvm;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;

namespace oprot.plot.core
{
    public class GraphFeatureViewModel:ObservableObject
    {
        public event Action CurveChanged;//create new graph element
        public event Action CurveInvalidated;//force update on existing graph element

        private GraphFeatureKind _curveType = GraphFeatureKind.IECStandardInverse;
        private GraphFeature _curveObject;

        public GraphFeatureKind CurveType
        {
            get
            {
                return _curveType;
            }
            set
            {
                _curveType = value;
                SetNewCurve();
                RaisePropertyChanged(nameof(CurveType));
                RaisePropertyChanged(nameof(CurveObject));
                RaiseCurveChanged();
            }
        }

        public GraphFeature CurveObject
        {
            get
            {
                return _curveObject;
            }
        }

        public GraphFeatureViewModel()
        {
            SetNewCurve();
        }
        
        private void RaiseCurveChanged()
        {
            CurveChanged?.Invoke();
        }

        private void RaiseCurveInvalidated()
        {
            CurveInvalidated?.Invoke();
        }
        
        private void SetNewCurve()
        {
            switch (_curveType)
            {
                case GraphFeatureKind.DefiniteTime:
                    _curveObject = new DefiniteTimeCharacteristic(_curveObject);
                    break;
                case GraphFeatureKind.IECStandardInverse:
                    _curveObject = new IECStandardInverse(_curveObject);
                    break;
                case GraphFeatureKind.IECVeryInverse:
                    _curveObject = new IECVeryInverse(_curveObject);
                    break;
                case GraphFeatureKind.IECExtremelyInverse:
                    _curveObject = new IECExtremelyInverse(_curveObject);
                    break;
                case GraphFeatureKind.IEEEModeratelyInverse:
                    _curveObject = new IEEEModeratelyInverse(_curveObject);
                    break;
                case GraphFeatureKind.IEEEVeryInverse:
                    _curveObject = new IEEEVeryInverse(_curveObject);
                    break;
                case GraphFeatureKind.IEEEExtremelyInverse:
                    _curveObject = new IEEEExtremelyInverse(_curveObject);
                    break;
                case GraphFeatureKind.SandCPositrolFuseTypeK:
                    _curveObject = new SandCFuseK(_curveObject);
                    break;
                case GraphFeatureKind.SandCPositrolFuseTypeT:
                    _curveObject = new SandCFuseT(_curveObject);
                    break;
                case GraphFeatureKind.ChanceFuseTypeK:
                    _curveObject = new ChanceFuseK(_curveObject);
                    break;
                case GraphFeatureKind.ChanceFuseTypeT:
                    _curveObject = new ChanceFuseT(_curveObject);
                    break;
                case GraphFeatureKind.FuseSaver:
                    _curveObject = new FuseSaver(_curveObject);
                    break;
                case GraphFeatureKind.TripSaver:
                    _curveObject = new TripSaver(_curveObject);
                    break;
                case GraphFeatureKind.NHgGFuse690V:
                    _curveObject = new NHFuse(_curveObject);
                    break;
                case GraphFeatureKind.HRCBoltedFuse:
                    _curveObject = new HRCBoltedFuse(_curveObject);
                    break;
                case GraphFeatureKind.HRCKnifeFuse:
                    _curveObject = new HRCKnifeFuse(_curveObject);
                    break;
                case GraphFeatureKind.HRCMJTypeFuse:
                    _curveObject = new HRCMJ30Fuse(_curveObject);
                    break;
                case GraphFeatureKind.ABBCEF:
                    _curveObject = new ABBCEFFuse(_curveObject);
                    break;
                case GraphFeatureKind.FaultLevelAnnotation:
                    _curveObject = new FaultLevelAnnotation(_curveObject);
                    break;
            }
            _curveObject.PropertyChanged += _curveObject_PropertyChanged;
        }

        private void _curveObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaiseCurveChanged();
        }

        public object Clone()
        {
            GraphFeatureViewModel obj = (GraphFeatureViewModel)this.MemberwiseClone();
            obj.CloneClean();
            return obj;
        }

        private void CloneClean()
        {
            CurveChanged = null;
            CurveInvalidated = null;
            
            _curveObject = (GraphFeature)_curveObject.Clone();
            _curveObject.PropertyChanged += _curveObject_PropertyChanged;
        }

        

    }
}
