using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using oprot.database.core;
using System.Data.Entity;

namespace oprot.database.viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private core.DatabaseContext _context = new core.DatabaseContext();
        private CollectionViewSource protectedPlantViewSource;
        private CollectionViewSource protectedPlantProtectionRelaysViewSource;
        private CollectionViewSource protectedPlantProtectionRelaysProtectionElementsViewSource;

        public MainWindow()
        {
            InitializeComponent();

            protectedPlantViewSource = ((CollectionViewSource)(FindResource("protectedPlantViewSource")));
            protectedPlantProtectionRelaysViewSource = ((CollectionViewSource)(FindResource("protectedPlantProtectionRelaysViewSource")));
            protectedPlantProtectionRelaysProtectionElementsViewSource = ((CollectionViewSource)(FindResource("protectedPlantProtectionRelaysProtectionElementsViewSource")));
            DataContext = this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource protectedPlantViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("protectedPlantViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // protectedPlantViewSource.Source = [generic data source]
            _context.ProtectedPlant.Load();
            // After the data is loaded, call the DbSet<T>.Local property    
            // to use the DbSet<T> as a binding source.   
            protectedPlantViewSource.Source = _context.ProtectedPlant.Local;
        }
    }
}
