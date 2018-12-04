using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Input;

//Event Design: http://msdn.microsoft.com/en-us/library/ms229011.aspx

namespace MicroMvvm
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        protected void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (propertyName != null)
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void ClearNotifyChangedHandler()
        {
            this.PropertyChanged = null;
        }
    }
}
