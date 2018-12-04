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
        public DataWindow(ObservableCollection<GraphFeatureViewModel> curves)
        {
            InitializeComponent();
            _model.Curves = curves;
            _model.Generate();
        }
    }
}
