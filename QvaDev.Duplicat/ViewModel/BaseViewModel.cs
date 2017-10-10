using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Threading;
using QvaDev.Common.Annotations;

namespace QvaDev.Duplicat.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _propertyValues = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;
        public SynchronizationContext SynchronizationContext { get; set; }

        [NotifyPropertyChangedInvocator]
        protected T Get<T>(Func<T> defaultValueFactory = null, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                return default(T);

            if (!_propertyValues.ContainsKey(propertyName))
                _propertyValues[propertyName] = defaultValueFactory == null ? default(T) : defaultValueFactory.Invoke();

            return (T)_propertyValues[propertyName];
        }

        [NotifyPropertyChangedInvocator]
        protected void Set<T>(T value, bool raiseEvent = true, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName)) return;

            var oldValue = Get<T>(null, propertyName);
            if (value.Equals(oldValue)) return;

            _propertyValues[propertyName] = value;

            if(raiseEvent) OnPropertyChanged(propertyName);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            lock (SynchronizationContext)
            {
                SynchronizationContext.Post(o => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), null);
            }
        }
    }
}
