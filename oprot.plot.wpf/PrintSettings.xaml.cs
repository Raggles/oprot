using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace oprot.plot.wpf
{
    /// <summary>
    /// Interaction logic for PrintSettings.xaml
    /// </summary>
    public partial class PrintSettings : Window
    {
        MainViewModel _model = null;

        public PrintSettings(MainViewModel model)
        {
            InitializeComponent();
            _model = model;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string _previewWindowXaml =
            @"<Window
                xmlns                 ='http://schemas.microsoft.com/netfx/2007/xaml/presentation'
                xmlns:x               ='http://schemas.microsoft.com/winfx/2006/xaml'
                xmlns:local           ='clr-namespace:oprot.plot.wpf;assembly=oprot.plot.wpf' 
                Title                 ='Print Preview - @@TITLE'
                Height                ='200'
                Width                 ='300'
                WindowStartupLocation ='CenterOwner'>
                <local:DocumentViewer2 Name='dv1'/>
             </Window>";
            string pack = "pack://temp.xps";
            try
            {

                using (var ms = new MemoryStream())
                {


                    using (var pkg = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite))
                    {
                        PackageStore.AddPackage(new Uri(pack), pkg);
                        using (var doc = new XpsDocument(pkg, CompressionOption.SuperFast, pack))
                        {
                            //these are the defaults for A4 landscape
                            double width = 1122;//2970/3;//XPS documents are 96 units per inch
                            double height = 793;//2100/3;
                            int imageWidth = (int)width;
                            int imageHeight = (int)height;
                            if (radA3.IsChecked == true)
                            {
                                width *= 1.414;
                                height *= 1.414;
                            }
                            //swap them around
                            if (radPortrait.IsChecked == true)
                            {
                                double t = width;
                                int t2 = imageWidth;
                                width = height;
                                imageWidth = imageHeight;
                                height = t;
                                imageHeight = t2;
                            }
                            int resolution = 400;// radA4.IsChecked == true ? 400 : 50;
                            FixedPage fp = new FixedPage();
                            fp.Width = width;
                            fp.Height = height;
                            Grid g = new Grid
                            {
                                Width = width,
                                Height = height
                            };
                            fp.Children.Add(g);
                            imageWidth = (int)((imageWidth-20) * ((double)resolution / 96));
                            imageHeight = (int)((imageHeight-10) * (double)resolution / 96);
                            var pngExporter = new PngExporter { Width = imageWidth, Height = imageHeight, Background = OxyColors.White, Resolution = resolution };
                            Image img = new Image
                            {
                                Source = pngExporter.ExportToBitmap(_model.ProtectionPlot),
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                                Margin = new Thickness(0,10,10,10)

                            };
                            g.Children.Add(img);
                            var writer = XpsDocument.CreateXpsDocumentWriter(doc);
                            writer.Write(fp);
                            doc.Close();
                            pkg.Close();
                            PackageStore.RemovePackage(new Uri(pack));
                        }
                    }

                    ms.Position = 0;
                    using (var pkg = Package.Open(ms, FileMode.Open, FileAccess.Read))
                    {
                        PackageStore.AddPackage(new Uri(pack), pkg);
                        var d = new XpsDocument(pkg, CompressionOption.SuperFast, pack);
                        FixedDocumentSequence fds = d.GetFixedDocumentSequence();

                        string s = _previewWindowXaml;
                        s = s.Replace("@@TITLE", _model.PlotTitle);

                        using (var reader = new System.Xml.XmlTextReader(new StringReader(s)))
                        {
                            //XamlReader
                            Window preview = XamlReader.Load(reader) as Window;
                            preview.WindowState = WindowState.Maximized;
                            DocumentViewer2 dv1 = LogicalTreeHelper.FindLogicalNode(preview, "dv1") as DocumentViewer2;
                            dv1.PageSize = radA4.IsChecked == true ? System.Printing.PageMediaSizeName.ISOA4 : System.Printing.PageMediaSizeName.ISOA3;
                            dv1.Orientation = radLandscape.IsChecked == true ? System.Printing.PageOrientation.Landscape : System.Printing.PageOrientation.Portrait;
                            dv1.FitToHeight();
                            dv1.Document = fds as IDocumentPaginatorSource;
                            preview.ShowDialog();
                            Close();
                        }
                        PackageStore.RemovePackage(new Uri(pack));
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    PackageStore.RemovePackage(new Uri(pack));
                }
                catch { }
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
