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
    public class LogFunctionSeries : LineSeries
    {
        public List<DataPoint> Shadow = new List<DataPoint>();

        public double DiscriminationMargin { get; set; } = 0.2;

        public bool ShowDiscriminationMargin { get; set; } = true;

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

            if (ShowDiscriminationMargin)
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
                rc.DrawPolygon(polypoints, polyColour, ActualColor, 0.2, EdgeRenderingMode.PreferSharpness);
            }

            Color = OxyColors.Green;
            this.RenderPoints(rc, actualPoints);

            

            if (this.LabelFormatString != null)
            {
                // render point labels (not optimized for performance)
                this.RenderPointLabels(rc);
            }

            if (this.LineLegendPosition != LineLegendPosition.None && !string.IsNullOrEmpty(this.Title))
            {
                // renders a legend on the line
                //TODO:
                //this.RenderLegendOnLine(rc);
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="LogFunctionSeries" /> class using a function <code>f(x)</code>.
        /// The points are distributed equally between each order of magnitude to ensure a smooth curve
        /// </summary>
        /// <param name="f">The function <code>f(x)</code>.</param>
        /// <param name="x0">The start x value.</param>
        /// <param name="x1">The end x value.</param>
        /// <param name="n">The number of points.</param>
        /// <param name="title">The title (optional).</param>
        public LogFunctionSeries(Func<double, double> f, double x0, double x1, int n, string title = null, double discriminationMargin = 0.04, double scalingFactor = 1)
        {
            DiscriminationMargin = discriminationMargin;
            List<DataPoint> revShadow = new List<DataPoint>();
            this.Title = title;

            if (x0 <= 0)
                x0 = 0.01;

            double logX0 = Math.Log10(x0);
            double logX1 = Math.Log10(x1);
            double interval = (logX1 - logX0) / n;

            for (int i = 0; i < n; i++)
            {
                double x = Math.Pow(10, logX0 + interval * i);
                double y = f(x * scalingFactor);
                this.Points.Add(new DataPoint(x, y));
                this.Shadow.Add(new DataPoint(x, y + DiscriminationMargin));
                revShadow.Add(new DataPoint(x, Math.Max(y - DiscriminationMargin, 0.01)));
            }

            for (int i = revShadow.Count-1; i>= 0; i--)
            {
                this.Shadow.Add(revShadow[i]);
            }
        }
    }
}

