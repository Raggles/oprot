using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace oprot.plot.core
{
    public partial class CurrentModifier : NetworkObject
    {
        private string name;

        public override string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
    }
}
