using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.plot.core
{
    public class GradingHighlighter : GraphableFeature
    {
        public List<GradingResult> Result { get; set; } = new List<GradingResult>();
        protected override PlotElement GetPlotElement()
        {
            var s =  new GradingRegion(Result);
            s.Color = this.Color;
            return s;
        }
    }

    public class GradingRegion : LineSeries
    {

        public List<DataPoint> Regions = new List<DataPoint>();

        /// <summary>
        /// Initializes a new instance of the <see cref = "LogFunctionSeries" /> class.
        /// </summary>
        public GradingRegion() { }

        public GradingRegion(List<GradingResult> l)
        {
            foreach (var r in l)
            {
                foreach (var s in r.Sections)
                {
                    if (!s.Grades)
                    {
                        var dp = new DataPoint(s.From, s.To);
                        Regions.Add(dp);
                    }
                }
            }
        }


        /// <summary>
        /// Renders the series on the specified rendering context.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        public override void Render(IRenderContext rc)
        {
            var actualPoints = this.ActualPoints;
            if (actualPoints == null || actualPoints.Count == 0)
            {
                //return;
            }

            this.VerifyAxes();

            var clippingRect = this.GetClippingRect();
            rc.SetClip(clippingRect);


            for (int i = 0; i < Regions.Count; i++)
            {
                OxyRect r = new OxyRect(this.Transform(Regions[i].X, YAxis.ActualMinimum), this.Transform(Regions[i].Y, YAxis.ActualMaximum));
                
                OxyColor polyColour = ActualColor;
                polyColour = polyColour.ChangeIntensity(4);
                polyColour = polyColour.ChangeSaturation(0.25);
                polyColour = OxyColor.FromAColor(127, polyColour);
                rc.DrawClippedRectangle(clippingRect, r, polyColour, ActualColor, 0.5);

            }

            rc.ResetClip();
        }
    }
}
