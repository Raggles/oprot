using oprot.plot.core;
using System;

namespace oprot.plot.wpf.ViewModels
{
    public class OprotViewModelProvider : IViewModelProvider
    {
        private NetworkViewModel networkViewModel;

        public OprotViewModelProvider(NetworkViewModel networkViewModel)
        {
            this.networkViewModel = networkViewModel;
        }

        public TViewModel GetFor<TViewModel>(object m) where TViewModel : class
        {
            Type t = typeof(TViewModel);
            if (typeof(TViewModel) == typeof(NetworkObjectViewModel) && m is ProtectionDevice d)
            {
                return new ProtectionDeviceViewModel(d, this) as TViewModel;
            }
            if (typeof(TViewModel) == typeof(ProtectionElementViewModel) && m is ProtectionElement e)
            {
                return new ProtectionElementViewModel(e) as TViewModel;
            }
            return null;
        }
    }

}
