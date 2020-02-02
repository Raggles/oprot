using MicroMvvm;
using Newtonsoft.Json;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace oprot.plot.core
{
    public enum GraphFeatureKind
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
        ABBCEF
    }
    public abstract class GraphFeature : ObservableObject
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        protected static void NotifyStaticPropertyChanged([CallerMemberName] String propertyName = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        protected static double _maximumCurrent = 30000;// Properties.Settings.Default.MaximumCurrent;
        protected static double _minimumCurrent = 1;// Properties.Settings.Default.MinimumCurrent;
        protected static int _numberSamples = 1000;// Properties.Settings.Default.NumberOfSamples;
        protected static double _baseVoltage = 11000;// Properties.Settings.Default.BaseVoltage;
        private static List<GraphFeature> _graphFeatures = new List<GraphFeature>();

        public event Action CurveChanged;
        public event Action CurveInvalidated;

        protected string _name = "Name";
        protected double _voltage = 11000;// Properties.Settings.Default.BaseVoltage;
        protected OxyColor _color = OxyColors.Undefined;
        protected bool _hidden = false;
        protected double _tempMultiplier = 1.0;
        protected bool _appendCurveTypeToDisplayName = true;
        protected PlotElement _plotElement;

        public static double BaseVoltage
        {
            get
            {
                return _baseVoltage;
            }
            set
            {
                _baseVoltage = value;
                NotifyStaticPropertyChanged();
                UpdateAllCurves();
            }
        }

        public static int NumberOfSamples
        {
            get
            {
                return _numberSamples;
            }
            set
            {
                _numberSamples = value;
                UpdateAllCurves();
                NotifyStaticPropertyChanged();
            }
        }

        public static double MaximumCurrent
        {
            get
            {
                return _maximumCurrent;
            }
            set
            {
                _maximumCurrent = value;
                UpdateAllCurves();
                NotifyStaticPropertyChanged();
            }
        }

        public static double MinimumCurrent
        {
            get
            {
                return _minimumCurrent;
            }
            set
            {
                _minimumCurrent = value;
                UpdateAllCurves();
                NotifyStaticPropertyChanged();
            }
        }

        [JsonConverter(typeof(OxyColorJsonConverter))]
        public abstract OxyColor Color { get; set; }

        [JsonIgnore]
        public string Description
        {
            get
            {
                return $"{Name}{this}";
            }
        }

        public bool Hidden
        {
            get
            {
                return _hidden;
            }
            set
            {
                _hidden = value;
                RaisePropertyChanged(nameof(Hidden));
                RaisePropertyChanged(nameof(DisplayColor));               
                UpdateGraphElement();
            }
        }
        
        [JsonIgnore]
        public OxyColor DisplayColor
        {
            get
            {
                if (_hidden)
                    return OxyColor.FromArgb(255, 78, 78, 78);
                else
                    return Color;
            }
        }

        public double TempMultiplier
        {
            get
            {
                return _tempMultiplier;
            }
            set
            {
                _tempMultiplier = value;
                RaisePropertyChanged(nameof(TempMultiplier));
                UpdateGraphElement();
            }
        }

        public double Voltage
        {
            get
            {
                return _voltage;
            }
            set
            {
                _voltage = value;
                RaisePropertyChanged(nameof(Voltage));
                UpdateGraphElement();
            }
        }

        public GraphFeature()
        {
            Register(this);
        }

        public GraphFeature(GraphFeature g) : this()
        {
            if (g != null)
            {
                _name = g.Name;
                _voltage = g.Voltage;
                _color = g.Color;
                _hidden = g.Hidden;
                _tempMultiplier = g.TempMultiplier;
                _appendCurveTypeToDisplayName = g.AppendCurveTypeToDisplayName;
            }
        }

        ~GraphFeature()
        {
            Unregister(this);
        }

        public static void Register(GraphFeature g)
        {
            _graphFeatures.Add(g);
        }

        public static void Unregister(GraphFeature g)
        {
            _graphFeatures.Remove(g);
        }

        public static void UpdateAllCurves()
        {
            foreach (var g in _graphFeatures)
            {
                g.UpdateGraphElement();
            }
        }

        protected void UpdateGraphElement()
        {
            _plotElement = GetPlotElement();
            RaisePropertyChanged(nameof(GraphElement));
            CurveChanged?.Invoke();
        }

        protected void RaiseGraphElementInvalidated()
        {
            CurveInvalidated?.Invoke();
        }

        [JsonIgnore]
        public PlotElement GraphElement
        {
            get
            {
                if (_plotElement == null)
                {
                    _plotElement = GetPlotElement();
                }
                return _plotElement;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Description));
                RaisePropertyChanged(nameof(Name));
                UpdateGraphElement();
            }
        }

        public string DisplayName
        {
            get
            {
                if (AppendCurveTypeToDisplayName)
                {
                    return Description;
                }
                else
                {
                    return Name;
                }
            }
        }

        public bool AppendCurveTypeToDisplayName
        {
            get
            {
                return _appendCurveTypeToDisplayName;
            }
            set
            {
                _appendCurveTypeToDisplayName = value;
                RaisePropertyChanged(nameof(DisplayName));
                RaisePropertyChanged(nameof(AppendCurveTypeToDisplayName));
                UpdateGraphElement();
            }
        }

        public abstract PlotElement GetPlotElement();

        public override string ToString()
        {
            return "";
        }

        public object Clone()
        {
            ProtectionCharacteristic obj = (ProtectionCharacteristic)this.MemberwiseClone();
            obj.ClearNotifyChangedHandler();
            return obj;
        }

        void SetTempMultiplierExecute(object d)
        {
            TempMultiplier = double.Parse(d.ToString());
        }

        bool CanSetTempMultiplierExecute(object d)
        {
            return true;
        }

        [JsonIgnore]
        public ICommand SetTempMultiplier { get { return new oprot.plot.core.RelayCommand<object>(SetTempMultiplierExecute, CanSetTempMultiplierExecute); } }

    }
}
