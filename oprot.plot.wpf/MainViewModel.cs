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
using Microsoft.Win32;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot.Legends;
using LiteDB;

namespace oprot.plot.wpf
{

    public partial class MainViewModel : ObservableObject
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

        [ObservableProperty] private int _id;

        [ObservableProperty]
        private bool appendDescriptionToDisplayName = true;

        [ObservableProperty]
        private double baseVoltage = 11000;

        [ObservableProperty]
        private int numberOfSamples  = 1000;

        [ObservableProperty]
        private double maximumCurrent  = 30000;

        [ObservableProperty]
        private double minimumCurrent = 1;

        [ObservableProperty]
        //[property: BsonIgnore]
        private ObservableCollection<GraphFeature> graphFeatures = new();

        [property: JsonIgnore]
        [property: BsonIgnore]
        [ObservableProperty]
        private PlotModel protectionPlot;

        public string PlotTitle
        {
            get => ProtectionPlot.Title;
            set
            {
                ProtectionPlot.Title = value;
                ProtectionPlot.InvalidatePlot(false);
                OnPropertyChanged(nameof(PlotTitle));
            }
        }

        [ObservableProperty]
        private double xMin;

        [ObservableProperty]
        private double xMax;

        [ObservableProperty]
        private double yMin;

        [ObservableProperty]
        private double yMax;



        [property: JsonIgnore]
        [property: BsonIgnore]
        [ObservableProperty]
        private GraphFeature selectedFeature;

        [property: JsonIgnore]
        [property: BsonIgnore]
        [ObservableProperty]
        private bool isNameFocused;

        public MainViewModel()
        {
            ProtectionPlot = new PlotModel { Title = "Protection Plot", IsLegendVisible = true};

            ProtectionPlot.Legends.Add(new Legend()
            {
                LegendTitle = "Legend",
                LegendPosition = LegendPosition.RightTop,
            });

            ProtectionPlot.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Bottom, Minimum = 100, Maximum = 10000, MajorGridlineStyle = LineStyle.Solid, Title = "Current (A)", AbsoluteMaximum = 100e3, AbsoluteMinimum = 0.01 });
            ProtectionPlot.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, Minimum = 0.01, Maximum = 100, MajorGridlineStyle = LineStyle.Solid, Title = "Time (s)", AbsoluteMaximum = 1e5, AbsoluteMinimum = 0.1 });
        }

        /// <summary>
        /// Create a new default feature
        /// </summary>
        /// <returns></returns>
        private GraphFeature CreateNewFeature()
        {
            var g = new GraphFeature(this, CharacteristicType.IECStandardInverse) { Color = _defaultColors[_colourIndex++ % _defaultColors.Count]};
            g.GraphicChanged += Redraw;
            g.GraphicInvalidated += G_GraphicInvalidated;
            g.PropertyChanged += G_PropertyChanged;
            return g;
        }

        private void G_GraphicInvalidated()
        {
            protectionPlot.InvalidatePlot(false);
        }

        private void G_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GraphFeature.IsSelected) && ((GraphFeature)sender).IsSelected)
            {
                SelectedFeature = sender as GraphFeature;
            }
        }

        public void DeselectAll()
        {
            DeselectAll(GraphFeatures);
            SelectedFeature = null;
        }
        private void DeselectAll(IList<GraphFeature> l)
        {
            foreach (var f in l)
            {
                f.IsSelected = false;
                DeselectAll(f.ChildItems);
            }
        }

        private void Redraw()
        {
            //TODO: we should only remove\update the one that changed!
            ProtectionPlot.Series.Clear();
            ProtectionPlot.Annotations.Clear();
            foreach (var g in GraphFeatures)
            {
                RedrawFeature(g);
                
            }
            ProtectionPlot.InvalidatePlot(true);
        }

        private void RedrawFeature(GraphFeature f)
        {
            if (!f.Hidden)
            {
                if (f.Graphic is Annotation)
                {
                    
                    ProtectionPlot.Annotations.Add(f.Graphic as Annotation);
                }
                else
                {
                    ProtectionPlot.Series.Add(f.Graphic as LineSeries);
                }
            }
            foreach (var feature in f.ChildItems)
            {
                RedrawFeature(feature);
            }
        }

        #region Commands
        [property: BsonIgnore]
        [RelayCommand]
        void DeleteFeature(GraphFeature f)
        {
            try
            {
                //delete the selected item, and add its children to its parent
                if (f == null)
                    return;
                var p = GetParent(f, GraphFeatures);

                if (p == null)
                {
                    graphFeatures.Remove(f);
                    f.ChildItems.ToList().ForEach(x => graphFeatures.Add(x));
                }
                else
                {
                    p.ChildItems.Remove(f);
                    f.ChildItems.ToList().ForEach(x => p.ChildItems.Add(x));
                }
                DeselectAll();

                Redraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private GraphFeature GetParent(GraphFeature f, IList<GraphFeature> l)
        {
            if (l.Contains(f))
                return null;//this handles the top level case
            foreach (var g in l)
            {
                if (g.ChildItems.Contains(f))
                    return g;
                else
                {
                    var y = GetParent(f, g.ChildItems);
                    if (y != null) return y;
                }
            }
            return null;
        }
        /*
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
        */
        /*

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
        */
        [property: BsonIgnore]
        [RelayCommand]
        void AddFeature(GraphFeature g)
        {
            try
            {
                var f = CreateNewFeature();
                if (g == null)
                {
                    GraphFeatures.Add(f);
                }
                else
                {
                    g.ChildItems.Add(f);
                    g.IsExpanded = true;
                }
                f.IsSelected = true;
                IsNameFocused = true;
                IsNameFocused = false;


                Redraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        [property: BsonIgnore]
        [RelayCommand]
        void Save()
        {
            try
            {
                //todo
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        [property: BsonIgnore]
        [RelayCommand]
        void SaveAs()
        {
            try
            {
                SaveFileDialog d = new SaveFileDialog();
                if (d.ShowDialog() ?? false)
                {
                    try
                    {
                        // Open database (or create if doesn't exist)
                        using (var db = new LiteDatabase(d.FileName))
                        {
                            var col = db.GetCollection<MainViewModel>("oprot");
                            col.Insert(this);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                    var jsonSerializerSettings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        Formatting = Newtonsoft.Json.Formatting.Indented

                    };
                    //XMin = ProtectionPlot.Axes[0].ActualMinimum;
                    //XMax = ProtectionPlot.Axes[0].ActualMaximum;
                    //YMin = ProtectionPlot.Axes[1].ActualMinimum;
                    //YMax = ProtectionPlot.Axes[1].ActualMaximum;
                    var json = JsonConvert.SerializeObject(this, jsonSerializerSettings);
                    File.WriteAllText(d.FileName + ".json", json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        [property: BsonIgnore]
        [RelayCommand]
        void ExoprtImage()
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
                        var pngExporter = new OxyPlot.Wpf.PngExporter { Width = (int)w, Height = (int)h};
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


        /*
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
        */
        
        #endregion
        /*
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
        */
    }
}
