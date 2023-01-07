using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.plot.core
{
    public partial class ProtectionElement : ObservableObject
    {
        [ObservableProperty] ProtectionCharacteristic characteristic;
        [ObservableProperty] string name;
        [ObservableProperty] OperatingQuantity tripsFor;
        [ObservableProperty] bool excludeFromGrading;
    }
}
