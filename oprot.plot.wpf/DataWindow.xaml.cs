using oprot.plot.core;
using System.Collections.ObjectModel;
using System.Windows;

namespace oprot.plot.wpf
{
    /// <summary>
    /// Interaction logic for ChartWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {
        public DataWindow(ObservableCollection<GraphFeature> curves, PlotDetails d)
        {
            InitializeComponent();
            _model.Curves = curves;
            _model.Generate(d);
        }
    }
}
