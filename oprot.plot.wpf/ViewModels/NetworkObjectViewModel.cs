using CommunityToolkit.Mvvm.ComponentModel;
using oprot.plot.core;
using OxyPlot;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.plot.wpf.ViewModels
{
    public abstract partial  class NetworkObjectViewModel : ObservableObject, IViewModel<NetworkObject>
    {
        [ObservableProperty] protected VmCollection<NetworkObjectViewModel, NetworkObject> children;
        [ObservableProperty] protected NetworkObject model;
        [ObservableProperty] private bool isSelected;

        protected IViewModelProvider _provider;

        public string Name
        {
            get => model.Name;
            set => model.Name = value;
        }

        public NetworkObjectViewModel(IViewModelProvider p)
        {
            _provider = p;
        }

        //TODO: I don't think we really need this in the view model
        public NetworkObject Parent
        {
            get => model.Parent;
            set => model.Parent = value;
        }

        partial void OnModelChanged(NetworkObject value)
        {
            model.PropertyChanged += (o, e) =>
            {
                this.OnPropertyChanged(e.PropertyName);
            };
            Children = new VmCollection<NetworkObjectViewModel, NetworkObject>(model.Children, _provider);
            OnDerivedModelChanged(value);  
        }

        /// <summary>
        /// So we can handle the change event in subclasses
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnDerivedModelChanged(NetworkObject value) { }
    }

}
