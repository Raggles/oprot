using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace oprot.plot.core
{
    public abstract partial class NetworkObject : ObservableObject
    {
        [ObservableProperty] ObservableCollection<NetworkObject> children;
        [ObservableProperty] NetworkObject parent;

        public abstract string Name { get; set; }

        public NetworkObject()
        {
            Children = new ObservableCollection<NetworkObject>();
        }

        partial void OnChildrenChanged(ObservableCollection<NetworkObject> value)
        {
            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var i in e.NewItems)
                {
                    ((NetworkObject)i).Parent = this;
                }
            }
        }
    }
}
