using System.Collections.ObjectModel;
using MicroMvvm;
using System.Data;
using oprot.plot.core;

namespace oprot.plot.wpf
{
    public class DataViewModel : ObservableObject
    {
        private GraphFeatureViewModel _curve = new GraphFeatureViewModel();
        private DataTable _data;

        public DataTable Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                RaisePropertyChanged(nameof(Data));
            }
        }

        public ObservableCollection<GraphFeatureViewModel> Curves { get; set; } = new ObservableCollection<GraphFeatureViewModel>();

        public void Generate()
        {
            DataTable dt = new DataTable();
            var current = dt.Columns.Add("Current (A)", typeof(float));

            for (int i = 0; i < 10000; i++)
            {
                var row = dt.NewRow();
                row[0] = i;
                dt.Rows.Add(row);
            }

            foreach (var curve in Curves)
            {
                var c = curve.CurveObject as ProtectionCharacteristic;
                if (c == null)
                    continue;
                string name = c.Name.Replace(".", "") ;
                int j = 2;
                while (dt.Columns.Contains(name))
                {
                    name = string.Format("{0} ({1})", c.Name, j);
                    j++;
                }
                var col = dt.Columns.Add(name, typeof(float));

                for (int i = 0; i < 10000; i++)
                {
                    dt.Rows[i][col] = (float)c.Curve(i * GraphFeature.BaseVoltage / c.Voltage);
                }
            }
            Data = dt;
        }
    }
}
