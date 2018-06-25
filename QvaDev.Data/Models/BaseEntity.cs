using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Threading;
using QvaDev.Common.Annotations;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public abstract class BaseEntity : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _propertyValues = new Dictionary<string, object>();

        [Key]
        [Dapper.Contrib.Extensions.Key]
        [InvisibleColumn]
        public int Id { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(SynchronizationContext syncContext, string propertyName)
        {
            lock (syncContext)
            {
                syncContext.Post(o => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), null);
            }
        }

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

            if (raiseEvent) OnPropertyChanged(propertyName);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotMapped]
        [InvisibleColumn]
        public string DisplayMember => ToString();

        public override string ToString()
        {
            return Id == 0 ? "UNSAVED - " : Id.ToString();
        }
    }
}
