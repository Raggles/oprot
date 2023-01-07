using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.plot.core
{
    public partial class ProtectionDevice : NetworkObject
    {
        [ObservableProperty] ObservableCollection<ProtectionElement> elements;
        [ObservableProperty] double minimumFaultLevel;
        [ObservableProperty] double maximumFaultLevel;
        [ObservableProperty] double voltage;

        private string name;

        public override string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public ProtectionDevice()
        {
            Elements = new();
            Children = new();
        }
    }
}
