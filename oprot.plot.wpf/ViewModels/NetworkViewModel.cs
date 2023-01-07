using CommunityToolkit.Mvvm.ComponentModel;
using oprot.plot.core;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.plot.wpf.ViewModels
{
    public partial class NetworkViewModel : ObservableObject, IViewModel<Network>
    {
        [ObservableProperty] private Network model;
        [ObservableProperty] private VmCollection<NetworkObjectViewModel, NetworkObject> networkObjects;
        [ObservableProperty] private PlotModel plotModel;

        [ObservableProperty] IViewModelProvider _provider;
        public string Name
        {
            get { return model.Name; }
            set { model.Name = value; }
        }

        public double BaseVoltage
        {
            get => model.BaseVoltage;
            set => model.BaseVoltage = value;
        }

        public NetworkViewModel(Network n)
        {
            _provider = new OprotViewModelProvider(this);
            Model = n;

            //TODO: initialise the plotmodel

        }

        partial void OnModelChanged(Network value)
        {
            model.PropertyChanged += (o, e) =>
            {
                this.OnPropertyChanged(e.PropertyName);
            };
            NetworkObjects = new VmCollection<NetworkObjectViewModel, NetworkObject>(Model.NetworkObjects, _provider);
        }
    }
}
