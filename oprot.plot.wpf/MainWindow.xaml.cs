using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using OxyPlot.Wpf;
using OxyPlot;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.IO.Packaging;
using System.Windows.Xps.Serialization;
using System.Windows.Markup;
using System.Threading;

namespace oprot.plot.wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnAddCurve_Click(object sender, RoutedEventArgs e)
        {
            //TODO: this should be a command on the viewmodel
            ((MainViewModel)DataContext).AddNewCurve();
            listBox.SelectedItem = ((MainViewModel)DataContext).Curves[((MainViewModel)DataContext).Curves.Count - 1];
        }

        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mnuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            if (d.ShowDialog() ?? false)
            {
                var jsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Newtonsoft.Json.Formatting.Indented

                };
                var json = JsonConvert.SerializeObject(DataContext, jsonSerializerSettings);
                File.WriteAllText(d.FileName, json);
            }
        }

        private void mnuShowChart_Click(object sender, RoutedEventArgs e)
        {
            new DataWindow(((MainViewModel)DataContext).Curves).Show();
        }

        private void mnuOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog d = new OpenFileDialog();
                if (d.ShowDialog() ?? false)
                {
                    MainViewModel m = JsonConvert.DeserializeObject<MainViewModel>(File.ReadAllText(d.FileName));
                    DataContext = m;
                    m.OnDeserialize();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void mnuSavePlot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog d = new SaveFileDialog();
                d.Filter = "PNG files|*.png";
                if (d.ShowDialog() ?? false)
                {
                    double w = plotView.ActualWidth;
                    double h = plotView.ActualHeight;
                    var pngExporter = new PngExporter { Width = (int)w, Height = (int)h, Background = OxyColors.White };
                    pngExporter.ExportToFile(((MainViewModel)DataContext).ProtectionPlot, d.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void mnuClear_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainViewModel();
        }

        private void mnuPrint_click(object sender, RoutedEventArgs e)
        {
            new PrintSettings(DataContext as MainViewModel).ShowDialog();
        }

        private void mnuCopy_Click(object sender, RoutedEventArgs e)
        {
            double w = plotView.ActualWidth;
            double h = plotView.ActualHeight;
            var pngExporter = new PngExporter { Width = (int)w, Height = (int)h, Background = OxyColors.White };
            Clipboard.SetImage(pngExporter.ExportToBitmap(((MainViewModel)DataContext).ProtectionPlot));
        }
    }
}
