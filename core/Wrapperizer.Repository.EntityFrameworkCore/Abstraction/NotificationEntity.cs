using System.ComponentModel;
using System.Runtime.CompilerServices;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Repository.EntityFrameworkCore.Abstraction
{
    public abstract class NotificationEntity<T> : Entity<T> , INotifyPropertyChanged , INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify<TV>(TV value, ref TV field, [CallerMemberName] string propertyName = "")
        {
            if (Equals(field, value)) return;
            
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        protected void NotifyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
        
        protected void NotifyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
