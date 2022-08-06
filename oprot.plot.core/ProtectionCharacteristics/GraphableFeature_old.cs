using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace oprot.plot.core
{
    /// <summary>
    /// The GraphableFeature is the base class for anything that can be displayed on a protection plot
    /// </summary>
    public abstract class GraphableFeature_old
    {
        //protected PlotElement _plotElement;
        //protected PlotDetails _plotDetails = new PlotDetails();
        
       


        /// <summary>
        /// The nominal color to draw this feature on the graph
        /// </summary>
        //[JsonConverter(typeof(OxyColorJsonConverter))]
        //public OxyColor Color { get; set; } = OxyColors.Undefined;

        /// <summary>
        /// Information about the graph that the feature will be drawn on.  This can be used to draw more efficiently.
        /// </summary>
        /*
        [JsonIgnore]
        public PlotDetails PlotParameters
        {
            get
            {
                return _plotDetails;
            }
            set
            {
                _plotDetails = value;
                _plotDetails.PropertyChanged += GraphableFeature_PropertyChanged;
            }
        }*/

        /// <summary>
        /// The description of the feature
        /// </summary>
        [JsonIgnore]
        public string Description => $"{this}";
            
        /*
        /// <summary>
        /// Hides the feature from the graph
        /// </summary>
        public bool Hidden { get; set; }
        /*

        /*
        /// <summary>
        /// The actual color used to draw the feature on the graph
        /// </summary>
        [JsonIgnore]
        public OxyColor DisplayColor => Hidden ? OxyColor.FromArgb(255, 78, 78, 78) : Color;
        */
        /// <summary>
        /// The name of this feature
        /// </summary>
        public string Name { get; set; } = "Name";

        /*
        /// <summary>
        /// The PlotElement for the feature to draw on the graph
        /// </summary>
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

        */

        /*
        /// <summary>
        /// The display name to show on the graph
        /// </summary>
        [JsonIgnore]
        public string DisplayName => PlotParameters?.AppendDescriptionToDisplayName ?? true ? $"{Name} ({Description})" : Name;
          */      
        /// <summary>
        /// Raised the the feature parameters have changed, and will need to be redrawn, regraded etc
        /// </summary>
        public event Action FeatureChanged;

        /*
        public GraphableFeature() {
            this.PropertyChanged += GraphableFeature_PropertyChanged;
        }

        private void GraphableFeature_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //whenever any feature changes, recreate the plot element and raise the event
            //not all property changes will require a redraw, but this makes things easy
            //_plotElement = GetPlotElement();
            RaiseFeatureChanged();
        }

        /// <summary>
        /// Returns a PlotElement to be drawn on the graph
        /// </summary>
        /// <returns></returns>
        //protected abstract PlotElement GetPlotElement();

        /// <summary>
        /// Creates a feature from another feature, resuing common parameters if possible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static T FromOther<T>(GraphableFeature f) where T : GraphableFeature, new()
        {
            T t = new T();
            t.PretendCopyConstructor(f);
            return t;
        }

        /// <summary>
        /// Copys common parameters in f to this feature.  
        /// Subclasses should override this function to copy addition common parameters, then call base.PretendCopyConstructor()
        /// </summary>
        /// <param name="f"></param>
        

        /// <summary>
        /// Raise the FeatureChanged event
        /// </summary>
        protected void RaiseFeatureChanged()
        {
            FeatureChanged?.Invoke();
        }

        /// <summary>
        /// Retrun the description of the feature
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "";
        }

        */
    }
}
