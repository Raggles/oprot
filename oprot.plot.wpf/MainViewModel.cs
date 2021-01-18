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
using System.Windows.Data;
using System.Globalization;
using Microsoft.Win32;
using System.IO;
using System.Linq;

namespace oprot.plot.wpf
{

    [ValueConversion(typeof(bool), typeof(bool))]
    public class NullBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return true;
            }
            return false;
        }
    }

    public class FeatureGroup : ObservableObject
    {
        public string Name { get; set; } = "Name";
        public ObservableCollection<GraphFeature> Features { get; set; } = new ObservableCollection<GraphFeature>();
        public bool Hidden { get; set; } = false;
        public bool Expanded { get; set; }

        //used to trigger regrading of curves
        public event Action MemberCurveChanged;
    }

    public class MainViewModel : ObservableObject
    {
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

        public PlotDetails PlotDetails { get; set; } = new PlotDetails();
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

        public ObservableCollection<FeatureGroup> Groups { get; set; } = new ObservableCollection<FeatureGroup>();

        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }

        [JsonIgnore]
        public PlotModel ProtectionPlot { get; private set; }

        [JsonIgnore]
        public GraphFeature SelectedFeature { get; set; }

        [JsonIgnore]
        public FeatureGroup SelectedGroup { get; set; }

        public MainViewModel()
        {
            PlotDetails.PropertyChanged += PlotDetails_PropertyChanged;
            ProtectionPlot = new PlotModel { Title = "Protection Plot" };
            ProtectionPlot.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Bottom, Minimum = 100, Maximum = 10000, MajorGridlineStyle = LineStyle.Solid, Title = "Current (A)", AbsoluteMaximum = 100e3, AbsoluteMinimum = 0.01 });
            ProtectionPlot.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, Minimum = 0.01, Maximum = 100, MajorGridlineStyle = LineStyle.Solid, Title = "Time (s)", AbsoluteMaximum = 1e5, AbsoluteMinimum = 0.1 });
        }

        private void PlotDetails_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Redraw();
        }

        public GraphFeature AddNewFeature(FeatureGroup g, GraphFeature newFeature = null, int position = -1)
        {
            if (newFeature == null)
            {
                newFeature = CreateNewFeature();
            }
            return AddTheFeature(g, newFeature, position);
        }

        /// <summary>
        /// Create a new default feature
        /// </summary>
        /// <returns></returns>
        private GraphFeature CreateNewFeature()
        {
            return CreateNewFeature(new IECStandardInverse() { Color = _defaultColors[_colourIndex++ % _defaultColors.Count] }, GraphFeatureKind.IECStandardInverse);
        }

        private GraphFeature CreateNewFeature(GraphableFeature f, GraphFeatureKind k)
        {
            GraphFeature g = new GraphFeature(f, k);
            g.GraphFeatureChanged += Redraw;
            g.Feature.PlotParameters = PlotDetails;
            return g;
        }

        private GraphFeature AddTheFeature(FeatureGroup g, GraphFeature newFeature, int position = -1)
        {
            g.Features.Add(newFeature);
            if (position >= 0 && position < g.Features.Count - 1)
            {
                g.Features.Move(g.Features.Count - 1, position);
            }

            if (newFeature.Feature.GraphElement is Annotation)
            {
                ProtectionPlot.Annotations.Add(newFeature.Feature.GraphElement as Annotation);
            }
            else
            {
                ProtectionPlot.Series.Add(newFeature.Feature.GraphElement as LineSeries);
            }
            ProtectionPlot.InvalidatePlot(false);
            return newFeature;
        }

        private void FeatureInvalidated()
        {
            ProtectionPlot.InvalidatePlot(false);
        }

        private void Redraw()
        {
            ProtectionPlot.Series.Clear();
            ProtectionPlot.Annotations.Clear();
            foreach (var g in Groups)
                foreach (var feature in g.Features)
                {
                    if (!feature.Feature.Hidden)
                    {

                        if (feature.Feature.GraphElement is Annotation)
                        {
                            ProtectionPlot.Annotations.Add(feature.Feature.GraphElement as Annotation);
                        }
                        else
                        {
                            ProtectionPlot.Series.Add(feature.Feature.GraphElement as LineSeries);
                        }

                    }
                }
            ProtectionPlot.InvalidatePlot(false);
        }

        #region Commands

        #region Delete Feature Command
        void DeleteFeatureExecute(GraphFeature f)
        {
            try
            {
                foreach (var g in Groups)
                {
                    g.Features.Remove(f);
                }
                Redraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanDeleteFeatureExecute(GraphFeature f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand DeleteFeature { get { return new MicroMvvm.RelayCommand<GraphFeature>(DeleteFeatureExecute, CanDeleteFeatureExecute); } }
        #endregion

        #region Duplicate Feature Command
        void DuplicateFeatureExecute(GraphFeature f)
        {
            try
            {
                var newFeature = (GraphFeature)f.Clone();
                newFeature.Feature.Name = "Copy of " + newFeature.Feature.Name;
                //TODO
                AddNewFeature(Groups[0], newFeature);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanDuplicateFeatureExecute(GraphFeature f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand DuplicateFeature { get { return new MicroMvvm.RelayCommand<GraphFeature>(DuplicateFeatureExecute, CanDuplicateFeatureExecute); } }
        #endregion

        #region Select Color Command
        void SelectColorExecute(GraphFeature f)
        {
            if (f == null)
                return;
            try
            {
                /* TODO:
                System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
                colorDialog.Color = System.Drawing.Color.FromArgb(f.Feature.Color.A, f.Feature.Color.R, f.Feature.Color.G, f.Feature.Color.B);
                colorDialog.AllowFullOpen = true;
                colorDialog.FullOpen = true;
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    f.Feature.Color = OxyColor.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                };
                */
            }
            catch { }
        }

        bool CanSelectColorExecute(GraphFeature f)
        {
            //TODO: sort out the relaycommand class
            return f != null;
            //return true;
        }

        [JsonIgnore]
        public ICommand SelectColor { get { return new MicroMvvm.RelayCommand<GraphFeature>(SelectColorExecute, CanSelectColorExecute); } }
        #endregion

        #region Move Feature Up Command
        void MoveFeatureUpExecute(GraphFeature f)
        {
            
            try
            {
                int i = SelectedGroup.Features.IndexOf(f);
                if (i > 0)
                    SelectedGroup.Features.Move(i, i - 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanMoveFeatureUpExecute(GraphFeature f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand MoveFeatureUp { get { return new MicroMvvm.RelayCommand<GraphFeature>(MoveFeatureUpExecute, CanMoveFeatureUpExecute); } }
        #endregion

        #region Move Feature Down Command
        void MoveFeatureDownExecute(GraphFeature f)
        { 
            try
            {
                int i = SelectedGroup.Features.IndexOf(f);
                if (i < SelectedGroup.Features.Count -1)
                    SelectedGroup.Features.Move(i, i + 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanMoveFeatureDownExecute(GraphFeature f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand MoveFeatureDown { get { return new MicroMvvm.RelayCommand<GraphFeature>(MoveFeatureDownExecute, CanMoveFeatureDownExecute); } }
        #endregion

        #region Copy Feature Json Command
        void CopyFeatureJsonExecute(GraphFeature f)
        {
            try
            {
                var jsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Newtonsoft.Json.Formatting.Indented

                };
                var json = JsonConvert.SerializeObject(f, jsonSerializerSettings);
                Clipboard.SetText(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanCopyFeatureJsonExecute(GraphFeature f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand CopyFeatureJson { get { return new MicroMvvm.RelayCommand<GraphFeature>(CopyFeatureJsonExecute, CanCopyFeatureJsonExecute); } }
        #endregion

        #region Copy Feature Base64 Command
        void CopyFeatureBase64Execute(GraphFeature f)
        {
            try
            {
                var jsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Newtonsoft.Json.Formatting.Indented

                };
                var json = JsonConvert.SerializeObject(f, jsonSerializerSettings);
                var b64 = System.Convert.ToBase64String(Util.Zip(json));
                Clipboard.SetText(b64);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanCopyFeatureBase64Execute(GraphFeature c)
        {
            return c != null;
        }

        [JsonIgnore]
        public ICommand CopyFeatureBase64 { get { return new MicroMvvm.RelayCommand<GraphFeature>(CopyFeatureBase64Execute, CanCopyFeatureBase64Execute); } }
        #endregion

        #region Paste Feature Command
        void PasteFeatureExecute(GraphFeature f)
        {
            try
            {
                string s = Clipboard.GetText();
                GraphFeature o;
                if (s[0] == '{')
                {
                    o = JsonConvert.DeserializeObject<GraphFeature>(s);
                }
                else
                {
                    var base64EncodedBytes = System.Convert.FromBase64String(s);
                    string s2 = Util.Unzip(base64EncodedBytes);
                    o = JsonConvert.DeserializeObject<GraphFeature>(s2);
                }
                AddNewFeature(Groups[0], o, Groups[0].Features.IndexOf(f) + 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanPasteFeatureExecute(GraphFeature f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand PasteFeature { get { return new MicroMvvm.RelayCommand<GraphFeature>(PasteFeatureExecute, CanPasteFeatureExecute); } }
        #endregion

        #region Add Feature Command
        void AddFeatureExecute(FeatureGroup f)
        {
            try
            {
                SelectedFeature = AddNewFeature(f);
                SelectedGroup.Expanded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanAddFeatureExecute(FeatureGroup f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand AddFeature { get { return new MicroMvvm.RelayCommand<FeatureGroup>(AddFeatureExecute, CanAddFeatureExecute); } }
        #endregion

        #region DeleteGroup Command
        void DeleteGroupExecute(FeatureGroup f)
        {
            try
            {
                Groups.Remove(f);
                Redraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanDeleteGroupExecute(FeatureGroup f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand DeleteGroup { get { return new MicroMvvm.RelayCommand<FeatureGroup>(DeleteGroupExecute, CanDeleteGroupExecute); } }
        #endregion

        #region Rename Group Command
        void RenameGroupExecute(FeatureGroup f)
        {
            try
            {
                f.Name = new RenameGroupWindow(f.Name).Go();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanRenameGroupExecute(FeatureGroup f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand RenameGroup { get { return new MicroMvvm.RelayCommand<FeatureGroup>(RenameGroupExecute, CanRenameGroupExecute); } }
        #endregion

        #region Add Group Command
        void AddGroupExecute(FeatureGroup f)
        {
            try
            {
                var g = new FeatureGroup() { Expanded = true };
                Groups.Add(g);
                SelectedGroup = g;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanAddGroupExecute(FeatureGroup f)
        {
            return true;
        }

        [JsonIgnore]
        public ICommand AddGroup { get { return new MicroMvvm.RelayCommand<FeatureGroup>(AddGroupExecute, CanAddGroupExecute); } }
        #endregion

        #region Duplicate Group Command
        void DuplicateGroupExecute(FeatureGroup f)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanDuplicateGroupExecute(FeatureGroup f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand DuplicateGroup { get { return new MicroMvvm.RelayCommand<FeatureGroup>(DuplicateGroupExecute, CanDuplicateGroupExecute); } }

        #endregion

        #region Move Group Up Command
        void MoveGroupUpExecute(FeatureGroup f)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanMoveGroupUpExecute(FeatureGroup f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand MoveGroupUp { get { return new MicroMvvm.RelayCommand<FeatureGroup>(MoveGroupUpExecute, CanMoveGroupUpExecute); } }
        #endregion

        #region Move Group DownCommand
        void MoveGroupDownExecute(FeatureGroup f)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanMoveGroupDownExecute(FeatureGroup f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand MoveGroupDown { get { return new MicroMvvm.RelayCommand<FeatureGroup>(MoveGroupDownExecute, CanMoveGroupDownExecute); } }
        #endregion

        #region Save Command
        void SaveExecute(FeatureGroup f)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanSaveExecute(FeatureGroup f)
        {
            return f != null;
        }

        [JsonIgnore]
        public ICommand Save { get { return new MicroMvvm.RelayCommand<FeatureGroup>(SaveExecute, CanSaveExecute); } }
        #endregion

        #region Save As Command
        void SaveAsExecute(FeatureGroup f)
        {
            try
            {
                SaveFileDialog d = new SaveFileDialog();
                if (d.ShowDialog() ?? false)
                {
                    var jsonSerializerSettings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        Formatting = Newtonsoft.Json.Formatting.Indented

                    };
                    XMin = ProtectionPlot.Axes[0].ActualMinimum;
                    XMax = ProtectionPlot.Axes[0].ActualMaximum;
                    YMin = ProtectionPlot.Axes[1].ActualMinimum;
                    YMax = ProtectionPlot.Axes[1].ActualMaximum;
                    var json = JsonConvert.SerializeObject(this, jsonSerializerSettings);
                    File.WriteAllText(d.FileName, json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanSaveAsExecute(FeatureGroup f)
        {
            return true;
        }

        [JsonIgnore]
        public ICommand SaveAs { get { return new MicroMvvm.RelayCommand<FeatureGroup>(SaveAsExecute, CanSaveAsExecute); } }
        #endregion

        #region Export Image Command
        void ExoprtImageExecute(FeatureGroup f)
        {
            try
            {
                try
                {
                    SaveFileDialog d = new SaveFileDialog();
                    d.Filter = "PNG files|*.png";
                    if (d.ShowDialog() ?? false)
                    {
                        double w = ProtectionPlot.PlotArea.Width;
                        double h = ProtectionPlot.PlotArea.Height;
                        var pngExporter = new OxyPlot.Wpf.PngExporter { Width = (int)w, Height = (int)h, Background = OxyColors.White };
                        OxyPlot.Wpf.ExporterExtensions.ExportToFile(pngExporter, ProtectionPlot, d.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanExoprtImageExecute(FeatureGroup f)
        {
            return true;
        }

        [JsonIgnore]
        public ICommand ExoprtImage { get { return new MicroMvvm.RelayCommand<FeatureGroup>(ExoprtImageExecute, CanExoprtImageExecute); } }
        #endregion

        #region Grader Command
        void GraderExecute(FeatureGroup f)
        {
            try
            {
                List<ProtectionCharacteristic> l = (from i in SelectedGroup.Features where i.Feature is ProtectionCharacteristic select (ProtectionCharacteristic)i.Feature).ToList();
                l.Sort();
                var r = Grader.Grade(l);
                string s = $"Grading Order (slowest>fastest):{Environment.NewLine}";
                for (int i = 0; i < l.Count; i++)
                {
                    s += "  -" + l[i].Name + Environment.NewLine;
                }
                foreach (var result in r)
                {
                    s += result.ToString() + Environment.NewLine;
                }

                var nf = CreateNewFeature(new GradingHighlighter() { Color = OxyColors.Red, Name = "Grading Result", Result = r, PlotParameters = PlotDetails }, GraphFeatureKind.GradingResult);
                AddNewFeature(SelectedGroup, nf);


                MessageBox.Show(s);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool CanGraderExecute(FeatureGroup f)
        {
            return f?.Features.Count > 1;
        }

        [JsonIgnore]
        public ICommand AutoGrader { get { return new MicroMvvm.RelayCommand<FeatureGroup>(GraderExecute, CanGraderExecute); } }
        #endregion

        
        #endregion
        public void OnDeserialize()
        {
            foreach (var g in Groups)
            {
                foreach (var feature in g.Features)
                {
                    feature.OnDeserialize();
                    feature.GraphFeatureChanged += Redraw;
                }
            }
            ProtectionPlot.Axes[0].Zoom(XMin, XMax);
            ProtectionPlot.Axes[1].Zoom(YMin, YMax);
            Redraw();
        }
    }
}
