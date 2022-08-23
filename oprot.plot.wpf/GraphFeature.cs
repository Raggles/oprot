using Newtonsoft.Json;
using System;
using System.Windows.Input;
using oprot.plot.core;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Security.Isolation;
using ABI.Windows.Services.Maps;
using OxyPlot;

namespace oprot.plot.wpf
{ 
    public enum FeatureType
        {
            IECStandardInverse,
            IECVeryInverse,
            IECExtremelyInverse,
            IEEEModeratelyInverse,
            IEEEVeryInverse,
            IEEEExtremelyInverse,
            DefiniteTime,
            SandCPositrolFuseTypeK,
            SandCPositrolFuseTypeT,
            ChanceFuseTypeK,
            ChanceFuseTypeT,
            FaultLevelAnnotation,
            FuseSaver,
            TripSaver,
            HRCKnifeFuse,
            HRCBoltedFuse,
            HRCMJTypeFuse,
            NHgGFuse690V,
            ABBCEF,
            GradingResult
        }
    public partial class GraphFeature : ObservableObject
    {

        [ObservableProperty] private PlotElement graphic;

        [ObservableProperty] private ProtectionCharacteristic feature;

        [ObservableProperty] private bool hidden;

        [ObservableProperty] private bool isExpanded;

        [ObservableProperty] private bool isSelected;

        [ObservableProperty] private OxyColor color;
        
        [ObservableProperty] private MainViewModel owner;

        [ObservableProperty] private ObservableCollection<GraphFeature> childItems = new();

        [ObservableProperty] private FeatureType graphFeatureType = FeatureType.IECExtremelyInverse;

        [ObservableProperty] private bool showDiscriminationMargin;

        //complete re-draw when:
        //hidden changed
        //feature changed
        //feature property changed
        //whenever a curve parameter has changed, i.e. the plot element has been regenerated
        public event Action GraphicChanged;

        //re-render when:
        //discrimination margin is changed 
        //show discrimination is changed
        //color is changed
        //whenever the plot element has not changed, but required re-rendering.
        //e.g. the discrimination margin has been enabled/disabled
        public event Action GraphicInvalidated;

        
        public GraphFeature() {  }

        public GraphFeature(MainViewModel m, FeatureType t)
        {
            //_initialized = true;
            Owner = m;
            GraphFeatureType = t;
            //SetNewFeature();
            SetNewGraphic();
        }
        
        private void SetNewFeature()
        {
            switch (graphFeatureType)
            {
                case FeatureType.DefiniteTime:
                    Feature = new DefiniteTimeCharacteristic();
                    break;
                case FeatureType.IECStandardInverse:
                    Feature = new IECStandardInverse();
                    break;
                case FeatureType.IECVeryInverse:
                    Feature = new IECVeryInverse(); 
                    break;
                case FeatureType.IECExtremelyInverse:
                    Feature = new IECExtremelyInverse();
                    break;
                case FeatureType.IEEEModeratelyInverse:
                    Feature = new IEEEModeratelyInverse();
                    break;
                case FeatureType.IEEEVeryInverse:
                    Feature = new IEEEVeryInverse();
                    break;
                case FeatureType.IEEEExtremelyInverse:
                    Feature = new IEEEExtremelyInverse();
                    break;
                case FeatureType.SandCPositrolFuseTypeK:
                    Feature = new SandCFuseK();
                    break;
                case FeatureType.SandCPositrolFuseTypeT:
                    Feature = new SandCFuseT();
                    break;
                case FeatureType.ChanceFuseTypeK:
                    Feature = new ChanceFuseK();
                    break;
                case FeatureType.ChanceFuseTypeT:
                    Feature = new ChanceFuseT();
                    break;
                case FeatureType.FuseSaver:
                    Feature = new FuseSaver();
                    break;
                case FeatureType.TripSaver:
                    Feature = new TripSaver();
                    break;
                case FeatureType.NHgGFuse690V:
                    Feature = new NHFuse();
                    break;
                case FeatureType.HRCBoltedFuse:
                    Feature = new HRCBoltedFuse();
                    break;
                case FeatureType.HRCKnifeFuse:
                    Feature = new HRCKnifeFuse();
                    break;
                case FeatureType.HRCMJTypeFuse:
                    Feature = new HRCMJ30Fuse();
                    break;
                case FeatureType.ABBCEF:
                    Feature = new ABBCEFFuse();
                    break;
                //case FeatureType.FaultLevelAnnotation:
                //    _curve = GraphFeature.FromOther<FaultLevelAnnotation>(Feature);
                //    break;
            }
            Feature.PropertyChanged += Feature_PropertyChanged;
        }

        private void SetNewGraphic()
        {
            if (Feature is FixedMarginCharacteristic c)
            {
                var s = new LogFunctionSeries(Feature.Curve, Owner.MinimumCurrent, Owner.MaximumCurrent, Owner.NumberOfSamples, Feature.Name, c.DiscriminationMargin, Feature.TempMultiplier * Owner.BaseVoltage / Feature.Voltage)
                {
                    ShowDiscriminationMargin = ShowDiscriminationMargin,
                    Color = this.Color
                };
                if (Feature.TempMultiplier != 1.0)
                    s.LineStyle = LineStyle.Dash;
                Graphic = s;
                
            }
            else if (feature is FuseDualCharacteristic)
            {
                //TODO: this is backwards, we can rewrite this to give the correct info now
                var s = new FuseSeries(Feature as FuseDualCharacteristic, Owner.MinimumCurrent, Owner.MaximumCurrent, Owner.NumberOfSamples, Feature.Name, Feature.TempMultiplier * Owner.BaseVoltage / Feature.Voltage);
                s.Color = this.Color;
                if (Feature.TempMultiplier != 1.0)
                    s.LineStyle = LineStyle.Dash;
                Graphic = s;
            }
            GraphicChanged?.Invoke();
        }


        partial void OnGraphFeatureTypeChanged(FeatureType value)
        {
            Debug.Print(nameof(OnGraphFeatureTypeChanged));
            SetNewFeature();
        }

        partial void OnFeatureChanged(ProtectionCharacteristic value)
        {
            Debug.Print(nameof(OnFeatureChanged));
            SetNewGraphic();
        }

        private void Feature_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.Print(nameof(Feature_PropertyChanged));
            SetNewGraphic();
        }
        

        
        partial void OnColorChanged(OxyColor value)
        {
            GraphicInvalidated?.Invoke();
        }

        partial void OnShowDiscriminationMarginChanged(bool value)
        {
            GraphicInvalidated?.Invoke();
        }


        /* TODO:
public object Clone()
{
    GraphFeature obj = (GraphFeature)this.MemberwiseClone();
    obj.CloneClean();
    return obj;
}*/

        /*
        private void CloneClean()
        {
            GraphFeatureChanged = null;
            GraphFeatureInvalidated = null;

            Feature = (GraphableFeature)Feature.Clone();
            Feature.FeatureChanged += _curveObject_PropertyChanged;
        }
        */

    }
}
