using CommunityToolkit.Mvvm.ComponentModel;
using oprot.plot.core;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace oprot.plot.wpf.ViewModels
{
    public enum GraphicChangeType
    {
        New,//indicates that the graphic has been replaced by a new object
        Update //the graphic object is the same, but needs to be redrawn
    }
    public class GraphicChangedEventArgs
    {
        public GraphicChangeType ChangeType { get; set; }
        public PlotElement Graphic { get; set; }
    }
    
    public delegate GraphicChangedEventArgs GraphicChangedEventHandler(object sender, GraphicChangedEventArgs e);

    public partial class ProtectionElementViewModel: ObservableObject, IViewModel<ProtectionElement>
    {
        [ObservableProperty] private ProtectionElement model;
        [ObservableProperty] private PlotElement graphic;
        [ObservableProperty] private CharacteristicType charType;
        [ObservableProperty] private bool isSelected;
        [ObservableProperty] private bool hidden;
        [ObservableProperty] private bool showDiscriminationMargin;
        [ObservableProperty] private OxyColor color;

        private static Dictionary<Type, CharacteristicType> charTypes = new()
        {
            {typeof(ABBCEFFuse), CharacteristicType.ABBCEF },
            {typeof(ChanceFuseK), CharacteristicType.ChanceFuseTypeK},
            {typeof(ChanceFuseT), CharacteristicType.ChanceFuseTypeT},
            {typeof(DefiniteTimeCharacteristic), CharacteristicType.DefiniteTime},
            {typeof(FuseSaver), CharacteristicType.FuseSaver},
            {typeof(HRCBoltedFuse), CharacteristicType.HRCBoltedFuse},
            {typeof(HRCKnifeFuse), CharacteristicType.HRCKnifeFuse},
            {typeof(HRCMJ30Fuse), CharacteristicType.HRCMJTypeFuse},
            {typeof(IECExtremelyInverse), CharacteristicType.IECExtremelyInverse},
            {typeof(IECStandardInverse), CharacteristicType.IECStandardInverse},
            {typeof(IECVeryInverse), CharacteristicType.IECVeryInverse},
            {typeof(IEEEExtremelyInverse), CharacteristicType.IEEEExtremelyInverse},
            {typeof(IEEEModeratelyInverse), CharacteristicType.IEEEModeratelyInverse },
            {typeof(IEEEVeryInverse), CharacteristicType.IEEEVeryInverse},
            {typeof(NHFuse), CharacteristicType.NHgGFuse690V},
            {typeof(SandCFuseK), CharacteristicType.SandCPositrolFuseTypeK},
            {typeof(SandCFuseT), CharacteristicType.SandCPositrolFuseTypeT},
            {typeof(TripSaver), CharacteristicType.TripSaver}
        };

        public event GraphicChangedEventHandler GraphicChanged;

        //TODO: we need to keep track of this somehow
        public ProtectionDevice Parent { get; set; }
        public Network Network { get; set; }
        public string Name
        {
            get { return model.Name; }
            set { model.Name = value; }
        }

        public ProtectionCharacteristic Characteristic
        {
            get { return model.Characteristic; }
            set { model.Characteristic = value; }
        }

        public OperatingQuantity TripsFor
        {
            get { return model.TripsFor; }
            set { model.TripsFor = value; }
        }

        public bool ExcludeFromGrading
        {
            get => model.ExcludeFromGrading;
            set => model.ExcludeFromGrading = value;
        }
        public ProtectionElementViewModel(ProtectionElement e)
        {
            Model = e;
            CharType = charTypes[e.Characteristic.GetType()];
        }

        partial void OnModelChanged(ProtectionElement value)
        {
            model.PropertyChanged += (o, e) =>
            {
                this.OnPropertyChanged(e.PropertyName);
            };
            model.Characteristic.PropertyChanged += Characteristic_PropertyChanged;
        }

        private void Characteristic_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetNewGraphic()
        {
            /*
            if (Model.Characteristic is FixedMarginCharacteristic c)
            {
                var s = new LogFunctionSeries(c.Curve,Parent.MinimumFaultLevel, Parent.MaximumFaultLevel, 300, Name, c.DiscriminationMargin, c.TempMultiplier * c.BaseVoltage / Parent.Voltage)
                {
                    ShowDiscriminationMargin = ShowDiscriminationMargin,
                    Color = this.Color
                };
                if (Feature.TempMultiplier != 1.0)
                    s.LineStyle = LineStyle.Dash;
                Graphic = s;

            }
            else if (Model.Characteristic is FuseDualCharacteristic f)
            {
                //TODO: this is backwards, we can rewrite this to give the correct info now
                var s = new FuseSeries(Feature as FuseDualCharacteristic, Owner.MinimumCurrent, Owner.MaximumCurrent, Owner.NumberOfSamples, Feature.Name, Feature.TempMultiplier * Owner.BaseVoltage / Feature.Voltage);
                s.Color = this.Color;
                if (Feature.TempMultiplier != 1.0)
                    s.LineStyle = LineStyle.Dash;
                Graphic = s;
            }
            GraphicChanged?.Invoke(this, 
                new GraphicChangedEventArgs() 
                { 
                    ChangeType = GraphicChangeType.New,
                    Graphic = Graphic 
                });*/
        }

        partial void OnCharTypeChanged(CharacteristicType value)
        { 
            switch (value)
            {
                case CharacteristicType.DefiniteTime:
                    model.Characteristic = new DefiniteTimeCharacteristic();
                    break;
                case CharacteristicType.IECStandardInverse:
                    model.Characteristic = new IECStandardInverse();
                    break;
                case CharacteristicType.IECVeryInverse:
                    model.Characteristic = new IECVeryInverse();
                    break;
                case CharacteristicType.IECExtremelyInverse:
                    model.Characteristic = new IECExtremelyInverse();
                    break;
                case CharacteristicType.IEEEModeratelyInverse:
                    model.Characteristic = new IEEEModeratelyInverse();
                    break;
                case CharacteristicType.IEEEVeryInverse:
                    model.Characteristic = new IEEEVeryInverse();
                    break;
                case CharacteristicType.IEEEExtremelyInverse:
                    model.Characteristic = new IEEEExtremelyInverse();
                    break;
                case CharacteristicType.SandCPositrolFuseTypeK:
                    model.Characteristic = new SandCFuseK();
                    break;
                case CharacteristicType.SandCPositrolFuseTypeT:
                    model.Characteristic = new SandCFuseT();
                    break;
                case CharacteristicType.ChanceFuseTypeK:
                    model.Characteristic = new ChanceFuseK();
                    break;
                case CharacteristicType.ChanceFuseTypeT:
                    model.Characteristic = new ChanceFuseT();
                    break;
                case CharacteristicType.FuseSaver:
                    model.Characteristic = new FuseSaver();
                    break;
                case CharacteristicType.TripSaver:
                    model.Characteristic = new TripSaver();
                    break;
                case CharacteristicType.NHgGFuse690V:
                    model.Characteristic = new NHFuse();
                    break;
                case CharacteristicType.HRCBoltedFuse:
                    model.Characteristic = new HRCBoltedFuse();
                    break;
                case CharacteristicType.HRCKnifeFuse:
                    model.Characteristic = new HRCKnifeFuse();
                    break;
                case CharacteristicType.HRCMJTypeFuse:
                    model.Characteristic = new HRCMJ30Fuse();
                    break;
                case CharacteristicType.ABBCEF:
                    model.Characteristic = new ABBCEFFuse();
                    break;
            
            }
        }
    }
}
