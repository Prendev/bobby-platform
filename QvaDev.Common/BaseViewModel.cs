using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QvaDev.Common.Annotations;

namespace QvaDev.Common
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _propertyValues = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                return default(T);

            if (!_propertyValues.ContainsKey(propertyName))
                _propertyValues[propertyName] = default(T);

            return (T)_propertyValues[propertyName];
        }

        [NotifyPropertyChangedInvocator]
        protected void Set<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName)) return;

            var oldValue = Get<T>(propertyName);
            if (value.Equals(oldValue)) return;

            _propertyValues[propertyName] = value;
            OnPropertyChanged(propertyName);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
