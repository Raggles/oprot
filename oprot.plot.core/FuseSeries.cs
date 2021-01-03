using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.plot.core
{

    /// <summary>
    /// Represents a line series that generates its dataset from a function.
    /// </summary>
    /// <remarks>Define <code>f(x)</code> and make a plot on the range <code>[x0,x1]</code> or define <code>x(t)</code> and <code>y(t)</code> and make a plot on the range <code>[t0,t1]</code>.</remarks>
    public class FuseSeries : LineSeries
    {
        public List<DataPoint> Shadow = new List<DataPoint>();

        public bool ShowDiscriminationMargin { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref = "LogFunctionSeries" /> class.
        /// </summary>
        public FuseSeries() { }

        /// <summary>
        /// Renders the series on the specified rendering context.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        public override void Render(IRenderContext rc)
        {
            var actualPoints = this.ActualPoints;
            if (actualPoints == null || actualPoints.Count == 0)
            {
                return;
            }

            this.VerifyAxes();

            var clippingRect = this.GetClippingRect();
            rc.SetClip(clippingRect);

            //if (ShowDiscriminationMargin)
            {
                List<ScreenPoint> polypoints = new List<ScreenPoint>();
                for (int i = 0; i < Shadow.Count; i++)
                {
                    polypoints.Add(this.Transform(Shadow[i]));
                }
                OxyColor polyColour = ActualColor;
                polyColour = polyColour.ChangeIntensity(4);
                polyColour = polyColour.ChangeSaturation(0.25);
                polyColour = OxyColor.FromAColor(127, polyColour);
                rc.DrawClippedPolygon(clippingRect, polypoints, 0.1, polyColour, ActualColor, 0.5);
            }

            this.RenderPoints(rc, clippingRect, actualPoints);



            if (this.LabelFormatString != null)
            {
                // render point labels (not optimized for performance)
                this.RenderPointLabels(rc, clippingRect);
            }

            rc.ResetClip();

            if (this.LineLegendPosition != LineLegendPosition.None && !string.IsNullOrEmpty(this.Title))
            {
                // renders a legend on the line
                //TODO
                //this.RenderLegendOnLine(rc);
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="LogFunctionSeries" /> class using a function <code>f(x)</code>.
        /// The points are distributed equally between each order of magnitude to ensure a smooth curve
        /// </summary>
        /// <param name="f">The fuse object<code>f(x)</code>.</param>
        /// <param name="x0">The start x value.</param>
        /// <param name="x1">The end x value.</param>
        /// <param name="n">The number of points.</param>
        /// <param name="title">The title (optional).</param>
        public FuseSeries(FuseDualCharacteristic f, double x0, double x1, int n, string title = null, double scalingFactor = 1)
        {
            List<DataPoint> maxClear = new List<DataPoint>();
            this.Title = title;

            var f1 = f.GetCurve(f.FuseSize, FuseCurveType.MinimumMeltingTime);
            var f2 = f.GetCurve(f.FuseSize, FuseCurveType.MaximumClearingTime);

            if (x0 <= 0)
                x0 = 0.01;

            double logX0 = Math.Log10(x0);
            double logX1 = Math.Log10(x1);
            double interval = (logX1 - logX0) / n;

            for (int i = 0; i < n; i++)
            {
                double x = Math.Pow(10, logX0 + interval * i);
                double y1 = f1(x * scalingFactor);
                double y2 = f2(x * scalingFactor);
                //this is the discrimination margin, formula from altom protection guide
                this.Points.Add(new DataPoint(x, y2 + 0.4*y2+0.15));
                Shadow.Add(new DataPoint(x, y1));
                maxClear.Add(new DataPoint(x, y2));
            }

            for (int i = maxClear.Count - 1; i >= 0; i--)
            {
                this.Shadow.Add(maxClear[i]);
            }
        }
    }
}

