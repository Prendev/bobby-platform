using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TradeSystem.Common.Annotations;

namespace TradeSystem.Common
{
	public abstract class BaseNotifyPropertyChange : INotifyPropertyChanged
	{
		private readonly ConcurrentDictionary<string, object> _propertyValues = new ConcurrentDictionary<string, object>();
		private readonly Dictionary<string, object> _propertyPrevActions = new Dictionary<string, object>();
		private readonly Dictionary<string, object> _propertyPostActions = new Dictionary<string, object>();

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected T Get<T>(Func<T> defaultValueFactory = null, [CallerMemberName] string propertyName = null)
		{
			if (string.IsNullOrWhiteSpace(propertyName)) return default(T);
			return (T) _propertyValues.GetOrAdd(propertyName,
				p => defaultValueFactory == null ? default(T) : defaultValueFactory.Invoke());
		}

		[NotifyPropertyChangedInvocator]
		protected void Set<T>(T value, bool raiseEvent = true, [CallerMemberName] string propertyName = null)
		{
			if (string.IsNullOrWhiteSpace(propertyName)) return;

			var updated = false;
			_propertyValues.AddOrUpdate(propertyName, value, (p, oldValue) =>
			{
				if (value?.Equals(oldValue) == true) return oldValue;
				updated = true;

				if (_propertyPrevActions.TryGetValue(propertyName, out var prev) && prev is Action<T> prevAction)
					prevAction.Invoke((T) oldValue);
				if (_propertyPostActions.TryGetValue(propertyName, out var post) && post is Action<T> postAction)
					postAction.Invoke(value);

				return value;
			});

			if (raiseEvent && updated) OnPropertyChanged(propertyName);
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			DependecyManager.SynchronizationContext?.Post(o => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), null);
		}


		protected void SetAction<T>(string propertyName, Action<T> prevAction, Action<T> postAction)
		{
			if (string.IsNullOrWhiteSpace(propertyName)) return;
			if (prevAction != null) _propertyPrevActions[propertyName] = prevAction;
			if (postAction != null) _propertyPostActions[propertyName] = postAction;
		}
	}
}
