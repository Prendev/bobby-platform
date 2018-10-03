using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QvaDev.Common;
using QvaDev.Common.Annotations;

namespace QvaDev.Data.Models
{
	public abstract class BaseNotifyPropertyChange : INotifyPropertyChanged
	{
		private readonly Dictionary<string, object> _propertyValues = new Dictionary<string, object>();
		private readonly Dictionary<string, object> _propertyPrevActions = new Dictionary<string, object>();
		private readonly Dictionary<string, object> _propertyPostActions = new Dictionary<string, object>();

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected T Get<T>(Func<T> defaultValueFactory = null, [CallerMemberName] string propertyName = null)
		{
			if (string.IsNullOrWhiteSpace(propertyName))
				return default(T);

			if (!_propertyValues.ContainsKey(propertyName))
				_propertyValues[propertyName] = defaultValueFactory == null ? default(T) : defaultValueFactory.Invoke();

			return (T)_propertyValues[propertyName];
		}

		[NotifyPropertyChangedInvocator]
		protected void Set<T>(T value, [CallerMemberName] string propertyName = null)
		{
			if (string.IsNullOrWhiteSpace(propertyName)) return;

			var oldValue = Get<T>(null, propertyName);
			if (value?.Equals(oldValue) == true) return;

			{
				if (_propertyPrevActions.TryGetValue(propertyName, out var prev) && prev is Action<T> prevAction)
					prevAction.Invoke(oldValue);

				if (_propertyPostActions.TryGetValue(propertyName, out var post) && post is Action<T> postAction)
					postAction.Invoke(value);
			}

			_propertyValues[propertyName] = value;

			if (typeof(T).IsGenericTypeDefinition && typeof(T).GetGenericTypeDefinition() != typeof(List<>))
				OnPropertyChanged(propertyName);
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			DependecyManager.SynchronizationContext?.Post(
				o => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), null);
		}


		protected void SetAction<T>(string propertyName, Action<T> prevAction, Action<T> postAction)
		{
			if (string.IsNullOrWhiteSpace(propertyName)) return;
			if (prevAction != null) _propertyPrevActions[propertyName] = prevAction;
			if (postAction != null) _propertyPostActions[propertyName] = postAction;
		}
	}
}
