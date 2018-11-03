using System.ComponentModel;
using System.Windows;

namespace Grappachu.Apps.Movideo.Common
{
    public abstract class ObservableObject : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}