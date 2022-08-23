using System.Collections.ObjectModel;
using MicroMvvm;
using System.Data;
using oprot.plot.core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace oprot.plot.wpf
{
    public class DataViewModel : ObservableObject
    {        
        public DataTable Data { get; set; }
        

        public ObservableCollection<GraphFeature> Curves { get; set; } = new ObservableCollection<GraphFeature>();

        public void Generate(double d)
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
                if (!(curve.Feature is ProtectionCharacteristic c))
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
                    dt.Rows[i][col] = (float)c.Curve(i * d / c.Voltage);
                }
            }
            Data = dt;
        }
    }
}
