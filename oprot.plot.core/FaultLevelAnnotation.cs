using System;
using System.Collections.Generic;
using System.Text;
using OxyPlot;
using OxyPlot.Annotations;

namespace oprot.plot.core
{
    public class FaultLevelAnnotation : GraphFeature
    {
        private double _current = 10000;
        public double Current
        {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
                RaisePropertyChanged(nameof(Current));
                UpdateGraphElement();
            }
        }

        public override OxyColor Color
        {
            get
            {
                return _plotElement == null ? _color : ((LineAnnotation)_plotElement).Color;
            }
            set
            {
                _color = value;
                if (_plotElement != null)
                    ((LineAnnotation)_plotElement).Color = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DisplayColor));
                RaiseGraphElementInvalidated();
            }
        }

        public override PlotElement GetPlotElement()
        {
            return new LineAnnotation { Type = LineAnnotationType.Vertical, X = Current * _voltage / _baseVoltage, MaximumY = 100000, StrokeThickness = 2, Color = _color, LineStyle = LineStyle.Dash, Text = Name };
        }

        public FaultLevelAnnotation(GraphFeature g = null) : base(g) { }
    }
}
