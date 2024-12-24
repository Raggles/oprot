using Newtonsoft.Json;
using System;
using System.Windows.Input;
using oprot.plot.core;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Printing;
using OxyPlot;
using System.Security;
using LiteDB;
using OxyPlot.Series;

namespace oprot.plot.wpf
{
    public partial class GraphFeature : ObservableObject
    {
        [property: BsonId] [ObservableProperty] private int graphFeatureId;

        [property:JsonIgnore]
        [property:BsonIgnore]
        [ObservableProperty] private PlotElement graphic;

        [property: BsonRef("characteristics")]
        [ObservableProperty] private ProtectionCharacteristic feature;

        [ObservableProperty] private bool hidden;

        [ObservableProperty] private bool isExpanded;

        [ObservableProperty] private bool isSelected;

        [property: BsonIgnore]

        [ObservableProperty] private OxyColor color;

        [property:BsonIgnore]
        [ObservableProperty] private MainViewModel owner;

        [property: BsonIgnore]
        [ObservableProperty] private ObservableCollection<GraphFeature> childItems = new();

        [property: BsonIgnore]

        [ObservableProperty] private CharacteristicType graphFeatureType = CharacteristicType.IECExtremelyInverse;

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

        public GraphFeature(MainViewModel m, CharacteristicType t)
        {
            //_initialized = true;
            Owner = m;
            GraphFeatureType = t;
            //SetNewFeature();
            SetNewGraphic();
        }
        
        private void SetNewFeature()
        {
            switch (GraphFeatureType)
            {
                case CharacteristicType.DefiniteTime:
                    Feature = new DefiniteTimeCharacteristic();
                    break;
                case CharacteristicType.IECStandardInverse:
                    Feature = new IECStandardInverse();
                    break;
                case CharacteristicType.IECVeryInverse:
                    Feature = new IECVeryInverse(); 
                    break;
                case CharacteristicType.IECExtremelyInverse:
                    Feature = new IECExtremelyInverse();
                    break;
                case CharacteristicType.IEEEModeratelyInverse:
                    Feature = new IEEEModeratelyInverse();
                    break;
                case CharacteristicType.IEEEVeryInverse:
                    Feature = new IEEEVeryInverse();
                    break;
                case CharacteristicType.IEEEExtremelyInverse:
                    Feature = new IEEEExtremelyInverse();
                    break;
                case CharacteristicType.SandCPositrolFuseTypeK:
                    Feature = new SandCFuseK();
                    break;
                case CharacteristicType.SandCPositrolFuseTypeT:
                    Feature = new SandCFuseT();
                    break;
                case CharacteristicType.ChanceFuseTypeK:
                    Feature = new ChanceFuseK();
                    break;
                case CharacteristicType.ChanceFuseTypeT:
                    Feature = new ChanceFuseT();
                    break;
                case CharacteristicType.FuseSaver:
                    Feature = new FuseSaver();
                    break;
                case CharacteristicType.TripSaver:
                    Feature = new TripSaver();
                    break;
                case CharacteristicType.NHgGFuse690V:
                    Feature = new NHFuse();
                    break;
                case CharacteristicType.HRCBoltedFuse:
                    Feature = new HRCBoltedFuse();
                    break;
                case CharacteristicType.HRCKnifeFuse:
                    Feature = new HRCKnifeFuse();
                    break;
                case CharacteristicType.HRCMJTypeFuse:
                    Feature = new HRCMJ30Fuse();
                    break;
                case CharacteristicType.ABBCEF:
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
            else if (Feature is FuseDualCharacteristic)
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

        partial void OnColorChanged(OxyColor value)
        {
            ((LineSeries)Graphic).Color = value;
            GraphicChanged?.Invoke();
        }
        partial void OnGraphFeatureTypeChanged(CharacteristicType value)
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

        partial void OnHiddenChanged(bool value)
        {
            GraphicChanged?.Invoke();
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
