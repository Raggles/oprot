using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MicroMvvm;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using OxyPlot.Annotations;
using OxyPlot.Series;
using oprot.plot.core;

namespace oprot.plot.wpf
{
    public class MainViewModel : ObservableObject
    {
        private GraphFeatureViewModel _curve = new GraphFeatureViewModel();
        private double _baseVoltage = Properties.Settings.Default.BaseVoltage;
        private List<OxyColor> _defaultColors = new List<OxyColor>
        {
            OxyColor.FromRgb(0x4E, 0x9A, 0x06),
            OxyColor.FromRgb(0xC8, 0x8D, 0x00),
            OxyColor.FromRgb(0xCC, 0x00, 0x00),
            OxyColor.FromRgb(0x20, 0x4A, 0x87),
            OxyColors.Red,
            OxyColors.Orange,
            OxyColors.Yellow,
            OxyColors.Green,
            OxyColors.Blue,
            OxyColors.Indigo,
            OxyColors.Violet
        };
        private int _colourIndex = 0;

        public double BaseVoltage {
            get
            {
                return _baseVoltage;
            }
            set
            {
                _baseVoltage = value;
                RaisePropertyChanged(nameof(BaseVoltage));
                foreach(var curve in Curves)
                {
                    //curve.UpdateBaseVoltage(value);
                    //TODO: fix this
                }
                Redraw();
            }
        }
        
        public string PlotTitle
        {
            get
            {
                return ProtectionPlot.Title;
            }
            set
            {
                ProtectionPlot.Title = value;
                ProtectionPlot.InvalidatePlot(false);
                RaisePropertyChanged(nameof(PlotTitle));
            }
        }

        [JsonIgnore]
        public PlotModel ProtectionPlot { get; private set; }
        public ObservableCollection<GraphFeatureViewModel> Curves { get; set; } = new ObservableCollection<GraphFeatureViewModel>();


        public MainViewModel()
        {
            ProtectionPlot = new PlotModel { Title = "Protection Plot" };
            ProtectionPlot.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Bottom, Minimum = 100, Maximum = 10000, MajorGridlineStyle = LineStyle.Solid, Title = "Current (A)", AbsoluteMaximum=100e3, AbsoluteMinimum=0.01 });
            ProtectionPlot.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, Minimum = 0.01, Maximum = 100, MajorGridlineStyle = LineStyle.Solid, Title = "Time (s)", AbsoluteMaximum = 1e5, AbsoluteMinimum=0.1 });
        }
                
        public void AddNewCurve(GraphFeatureViewModel newCurve = null, int position = -1)
        {
            if (newCurve == null)
                newCurve = new GraphFeatureViewModel();
            newCurve.CurveObject.Color = _defaultColors[_colourIndex++ % _defaultColors.Count];
            //newCurve.UpdateBaseVoltage(_baseVoltage);
            newCurve.CurveChanged += Redraw;
            newCurve.CurveInvalidated += CurveInvalidated;
            Curves.Add(newCurve);
            if (position >= 0 && position < Curves.Count - 1)
            {
                Curves.Move(Curves.Count - 1, position);
            }
            
            if (newCurve.CurveObject.GraphElement is Annotation)
            {
                ProtectionPlot.Annotations.Add(newCurve.CurveObject.GraphElement as Annotation);
            }
            else
            {
                ProtectionPlot.Series.Add(newCurve.CurveObject.GraphElement as LineSeries);
            }
            ProtectionPlot.InvalidatePlot(false);
        }

        private void CurveInvalidated()
        {
            ProtectionPlot.InvalidatePlot(false);
        }

        private void Redraw()
        {
            ProtectionPlot.Series.Clear();
            ProtectionPlot.Annotations.Clear();
            foreach (var curve in Curves)
            {
                if (!curve.CurveObject.Hidden)
                {

                    if (curve.CurveObject.GraphElement is Annotation)
                    {
                        ProtectionPlot.Annotations.Add(curve.CurveObject.GraphElement as Annotation);
                    }
                    else
                    {
                        ProtectionPlot.Series.Add(curve.CurveObject.GraphElement as LineSeries);
                    }

                }
            }           
            ProtectionPlot.InvalidatePlot(false);
        }

        #region Commands

        #region Delete Curve Command
        void DeleteCurveExecute(GraphFeatureViewModel c)
        {
            try
            {
                Curves.Remove(c);
                Redraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanDeleteCurveExecute(GraphFeatureViewModel c)
        {
            return c != null;
        }

        [JsonIgnore]
        public ICommand DeleteCurve { get { return new MicroMvvm.RelayCommand<GraphFeatureViewModel>(DeleteCurveExecute, CanDeleteCurveExecute); } }
        #endregion

        #region Duplicate Curve Command
        void DuplicateCurveExecute(GraphFeatureViewModel c)
        {
            try
            {
                var newCurve = (GraphFeatureViewModel)c.Clone();
                newCurve.CurveObject.Name = "Copy of " + newCurve.CurveObject.Name;
                AddNewCurve(newCurve);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanDuplicateCurveExecute(GraphFeatureViewModel c)
        {
            return c != null;
        }

        [JsonIgnore]
        public ICommand DuplicateCurve { get { return new MicroMvvm.RelayCommand<GraphFeatureViewModel>(DuplicateCurveExecute, CanDuplicateCurveExecute); } }
        #endregion

        #region Select Color Command
        void SelectColorExecute(GraphFeatureViewModel c)
        {
            if (c == null)
                return;
            try
            {
                System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
                colorDialog.Color = System.Drawing.Color.FromArgb(c.CurveObject.Color.A, c.CurveObject.Color.R, c.CurveObject.Color.G, c.CurveObject.Color.B);
                colorDialog.AllowFullOpen = true;
                colorDialog.FullOpen = true;
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    c.CurveObject.Color = OxyColor.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                };
            }
            catch { }
        }

        bool CanSelectColorExecute(GraphFeatureViewModel c)
        {
            //TODO: sort out the relaycommand class
            return c != null;
            //return true;
        }

        [JsonIgnore]
        public ICommand SelectColor { get { return new MicroMvvm.RelayCommand<GraphFeatureViewModel>(SelectColorExecute, CanSelectColorExecute); } }
        #endregion

        #region Move Curve Up Command
        void MoveCurveUpExecute(GraphFeatureViewModel c)
        {
            try
            {
                int i = Curves.IndexOf(c);
                if (i > 0)
                    Curves.Move(i, i - 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanMoveCurveUpExecute(GraphFeatureViewModel c)
        {
            return c != null;
        }

        [JsonIgnore]
        public ICommand MoveCurveUp { get { return new MicroMvvm.RelayCommand<GraphFeatureViewModel>(MoveCurveUpExecute, CanMoveCurveUpExecute); } }
        #endregion

        #region Move Curve Down Command
        void MoveCurveDownExecute(GraphFeatureViewModel c)
        {
            try
            {
                int i = Curves.IndexOf(c);
                if (i < Curves.Count -1)
                    Curves.Move(i, i + 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanMoveCurveDownExecute(GraphFeatureViewModel c)
        {
            return c != null;
        }

        [JsonIgnore]
        public ICommand MoveCurveDown { get { return new MicroMvvm.RelayCommand<GraphFeatureViewModel>(MoveCurveDownExecute, CanMoveCurveDownExecute); } }
        #endregion

        #region Copy Curve Json Command
        void CopyCurveJsonExecute(GraphFeatureViewModel c)
        {
            try
            {
                var jsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Newtonsoft.Json.Formatting.Indented

                };
                var json = JsonConvert.SerializeObject(c, jsonSerializerSettings);
                Clipboard.SetText(json);               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanCopyCurveJsonExecute(GraphFeatureViewModel c)
        {
            return c != null;
        }

        [JsonIgnore]
        public ICommand CopyCurveJson { get { return new MicroMvvm.RelayCommand<GraphFeatureViewModel>(CopyCurveJsonExecute, CanCopyCurveJsonExecute); } }
        #endregion

        #region Copy Curve Base64 Command
        void CopyCurveBase64Execute(GraphFeatureViewModel c)
        {
            try
            {
                var jsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Newtonsoft.Json.Formatting.Indented

                };
                var json = JsonConvert.SerializeObject(c, jsonSerializerSettings);
                var b64 =  System.Convert.ToBase64String(Util.Zip(json));
                Clipboard.SetText(b64);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanCopyCurveBase64Execute(GraphFeatureViewModel c)
        {
            return c != null;
        }

        [JsonIgnore]
        public ICommand CopyCurveBase64 { get { return new MicroMvvm.RelayCommand<GraphFeatureViewModel>(CopyCurveBase64Execute, CanCopyCurveBase64Execute); } }
        #endregion

        #region Paste Curve Command
        void PasteCurveExecute(GraphFeatureViewModel c)
        {
            try
            {
                string s = Clipboard.GetText();
                GraphFeatureViewModel o;
                if (s[0] == '{')
                {
                    o = JsonConvert.DeserializeObject<GraphFeatureViewModel>(s);
                }
                else
                {
                    var base64EncodedBytes = System.Convert.FromBase64String(s);
                    string s2 = Util.Unzip(base64EncodedBytes);
                    o = JsonConvert.DeserializeObject<GraphFeatureViewModel>(s2);
                }
                AddNewCurve(o, Curves.IndexOf(c) + 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanPasteCurveExecute(GraphFeatureViewModel c)
        {
            return c != null;
        }

        [JsonIgnore]
        public ICommand PasteCurve { get { return new MicroMvvm.RelayCommand<GraphFeatureViewModel>(PasteCurveExecute, CanPasteCurveExecute); } }
        #endregion

        #endregion
        public void OnDeserialize()
        {
            foreach(var curve in Curves)
            {
                curve.CurveChanged += Redraw;
                curve.CurveInvalidated += CurveInvalidated;
            }
            Redraw();
        }
    }
}
