using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.plot.core
{
    public  partial class Network :ObservableObject
    {
        [ObservableProperty] string name;
        [ObservableProperty] ObservableCollection<NetworkObject> networkObjects;
        [ObservableProperty] double baseVoltage;

        public Network()
        {
            NetworkObjects = new();
        }
    }
}
