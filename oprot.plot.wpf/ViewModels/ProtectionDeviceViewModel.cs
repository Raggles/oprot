using CommunityToolkit.Mvvm.ComponentModel;
using oprot.plot.core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.plot.wpf.ViewModels
{
    public partial class ProtectionDeviceViewModel : NetworkObjectViewModel
    {
        [ObservableProperty] private VmCollection<ProtectionElementViewModel, ProtectionElement> elements;
        private ProtectionDevice DerivedModel { get => Model as ProtectionDevice; }

        public double MinimumFaultLevel
        {
            get { return DerivedModel.MinimumFaultLevel; }
            set { DerivedModel.MinimumFaultLevel = value; }
        }

        public double MaximumFaultLevel
        {
            get { return DerivedModel.MaximumFaultLevel; }
            set { DerivedModel.MaximumFaultLevel = value; }
        }

        public ProtectionDeviceViewModel(ProtectionDevice d, IViewModelProvider p) : base(p)
        {
            Model = d;
        }

        protected override void OnDerivedModelChanged(NetworkObject value)
        {
            Elements = new VmCollection<ProtectionElementViewModel, ProtectionElement>(DerivedModel.Elements, _provider);
        }
    }
}
