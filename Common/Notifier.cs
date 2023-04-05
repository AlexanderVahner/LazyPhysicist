using System;
using System.ComponentModel;

namespace LazyPhysicist.Common
{
    public class Notifier : INotifyPropertyChanged
    {
        protected virtual void SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                NotifyPropertyChanged(propertyName);
            }
        }
        protected virtual void SetProperty<T>(Action<T> setValue, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            setValue(value);
            NotifyPropertyChanged(propertyName);
        }
        public void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
