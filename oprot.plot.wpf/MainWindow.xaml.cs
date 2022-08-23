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

        public MainWindow(string[] args)
        {
            InitializeComponent();

            try
            {
                MainViewModel m = JsonConvert.DeserializeObject<MainViewModel>(File.ReadAllText(args[0]));
                DataContext = m;
                //m.OnDeserialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mnuShowChart_Click(object sender, RoutedEventArgs e)
        {
            //TODO
            //new DataWindow(((MainViewModel)DataContext).Features, ((MainViewModel)DataContext).PlotDetails).Show();
        }

        private void mnuOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog d = new OpenFileDialog();
                if (d.ShowDialog() ?? false)
                {
                    MainViewModel m = JsonConvert.DeserializeObject<MainViewModel>(File.ReadAllText(d.FileName), new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    DataContext = m;
                    //m.OnDeserialize();
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
            var pngExporter = new PngExporter { Width = (int)w, Height = (int)h };
            Clipboard.SetImage(pngExporter.ExportToBitmap(((MainViewModel)DataContext).ProtectionPlot));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Disclaimer d = new Disclaimer();
            d.Owner = this;
            d.ShowDialog();
        }
    }
}
