using oprot.plot.core;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;
using System.Printing.IndexedProperties;
using oprot.plot.wpf.ViewModels;
using System.Diagnostics;
using System.Security.Policy;
using NuGet.Frameworks;

namespace oprot.Tests
{
    [TestClass]
    public class ViewModelTests
    {
        public (Network n, NetworkViewModel nvm) CreateNetwork()
        {
            Network n = new Network()
            {
                Name = "Test Network",
            };

            for (int i = 0; i < 10; i++)
            {
                ProtectionDevice d = new()
                {
                    Name = $"ProtectionDevice{i}",
                    MaximumFaultLevel = 1000,
                    MinimumFaultLevel = 500,
                };
                d.Elements.Add(new()
                {
                    Characteristic = new IECStandardInverse()
                    {
                        Name = $"Characteristic{i}",
                        HiSetPickup = 1000,
                        Tms = 1.0,
                        Pickup = 100
                    },
                    Name = $"Element{i}"
                });
                n.NetworkObjects.Add(d);
            }
            NetworkViewModel nvm = new NetworkViewModel(n);

            
            return (n, nvm);
        }

        public void CheckModelEqualsViewModel(Network n, NetworkViewModel nvm, bool printObjects = false)
        {
            PrintNetwork(n);
            PrintNetworkViewModel(nvm);
            Assert.IsTrue(nvm.Model == n);
            Assert.IsTrue(nvm.NetworkObjects.Count == n.NetworkObjects.Count);
            for (int i = 0; i < nvm.NetworkObjects.Count; i++)
            {
                Assert.IsTrue(nvm.NetworkObjects[i].Model == n.NetworkObjects[i]);
            }
        }

        [TestMethod]
        public void AddNetworkObjectToModel()
        {
            var n = CreateNetwork();
            n.n.NetworkObjects.Add(new ProtectionDevice() { Name = "NewNetworkObject" });
            CheckModelEqualsViewModel(n.n, n.nvm);
        }

        [TestMethod]
        public void AddNetworkObjectToViewModel()
        {
            var n = CreateNetwork();
            n.nvm.NetworkObjects.Add(new ProtectionDeviceViewModel(new ProtectionDevice(){ Name = "NewNetworkObjectVM" }, n.nvm.Provider));
            CheckModelEqualsViewModel(n.n, n.nvm);
        }

        [TestMethod]
        public void ChangeModelProperties()
        {
            var n = CreateNetwork();
            bool result = false;
            n.nvm.PropertyChanged += (s, e) => result = e.PropertyName == "Name";
            n.n.Name = "New Name";
            Assert.IsTrue(result);
            n.nvm.NetworkObjects[0].PropertyChanged += (s, e) => result = e.PropertyName == "Name";
            n.n.NetworkObjects[0].Name = "TestDevice";
            Assert.IsTrue(result);
            ((ProtectionDevice)n.n.NetworkObjects[0]).MinimumFaultLevel = 5;
            Assert.IsFalse(result);
        }

        private void N_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ViewModelCreationFromModel()
        {
            var n = CreateNetwork();
            CheckModelEqualsViewModel(n.n, n.nvm);
        }

        public void PrintNetwork(Network n)
        {
            Debug.WriteLine($"Network: {n.Name}");
            foreach(var d in n.NetworkObjects)
            {
                PrintNetworkObject(d);
            }
        }

        public void PrintNetworkObject(NetworkObject o)
        {
            Debug.WriteLine($"NetworkObject: {o.Name}");
            if (o is ProtectionDevice d)
            {
                foreach (var e in d.Elements)
                {
                    Debug.WriteLine($"Element: {e.Name}");
                }
            }
            foreach (var c in o.Children)
            {
                PrintNetworkObject(c);
            }
        }

        public void PrintNetworkObjectViewModel(NetworkObjectViewModel o)
        {
            Debug.WriteLine($"NetworkObjectVM: {o.Name}");
            if (o is ProtectionDeviceViewModel d)
            {
                foreach (var e in d.Elements)
                {
                    Debug.WriteLine($"ElementVM: {e.Name}");
                }
            }
            foreach (var c in o.Children)
            {
                PrintNetworkObjectViewModel(c);
            }
        }


        public void PrintNetworkViewModel(NetworkViewModel n)
        {
            Debug.WriteLine($"NetworkVM: {n.Name}");
            foreach (var d in n.NetworkObjects)
            {
                PrintNetworkObjectViewModel(d);
            }

        }
    }
}