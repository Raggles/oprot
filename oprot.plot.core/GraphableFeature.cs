using MicroMvvm;
using Newtonsoft.Json;
using OxyPlot;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace oprot.plot.core
{
    public abstract class GraphableFeature :ObservableObject
    {
        protected PlotElement _plotElement;

        public double TempMultiplier { get; set; } = 1.0;

        public double Voltage { get; set; } = 11000;


        [JsonConverter(typeof(OxyColorJsonConverter))]
        public OxyColor Color { get; set; } = OxyColors.Undefined;

        [JsonIgnore]
        public PlotDetails PlotParameters { get; set; } = new PlotDetails();

        [JsonIgnore]
        public string Description => $"{this}";
            
        public bool Hidden { get; set; }

        [JsonIgnore]
        public OxyColor DisplayColor => Hidden ? OxyColor.FromArgb(255, 78, 78, 78) : Color;

        public string Name { get; set; } = "Name";

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

        [JsonIgnore]
        public string DisplayName => PlotParameters.AppendDescriptionToDisplayName ? $"{Name} ({Description})" : Name;
                
        /// <summary>
        /// Raised the the curve parameters have changed, and will need to be redrawn, regraded etc
        /// </summary>
        public event Action FeatureChanged;

        public GraphableFeature() {
            this.PropertyChanged += GraphableFeature_PropertyChanged;
        }

        private void GraphableFeature_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _plotElement = GetPlotElement();
            RaiseFeatureChanged();
        }

        protected abstract PlotElement GetPlotElement();

        public static T FromOther<T>(GraphableFeature f) where T : GraphableFeature, new()
        {
            T t = new T();
            t.PretendCopyConstructor(f);
            return t;
        }

        protected virtual void PretendCopyConstructor(GraphableFeature f)
        {
            if (f != null)
            {
                Name = f.Name;
                Color = f.Color;
                Hidden = f.Hidden;
                Voltage = f.Voltage;
                TempMultiplier = f.TempMultiplier;
            }
        }

        protected void RaiseFeatureChanged()
        {
            FeatureChanged?.Invoke();
        }

        public override string ToString()
        {
            return "";
        }

        public object Clone()
        {
            GraphableFeature obj = (GraphableFeature)this.MemberwiseClone();
            obj._plotElement = null;
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
