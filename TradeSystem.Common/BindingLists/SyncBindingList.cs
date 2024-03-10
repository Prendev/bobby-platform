using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace TradeSystem.Common.BindingLists
{
	public class SyncBindingList<T> : BindingList<T> where T : class
	{
		private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

		/// <summary>
		/// Overrides the OnAddingNew method to ensure that modifications to the BindingList are performed on the main UI thread by using the captured synchronization context.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnAddingNew(AddingNewEventArgs e)
		{
			if (_syncContext == null)
				BaseAddingNew(e);
			else
				_syncContext.Send(state => BaseAddingNew(e), null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SyncBindingList{T}"/> class.
		/// </summary>
		public SyncBindingList()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SyncBindingList{T}"/> class.
		/// </summary>
		/// <param name="list">An <see cref="T:System.Collections.Generic.IList`1" /> of items to be contained in the <see cref="T:System.ComponentModel.BindingList`1" />.</param>
		public SyncBindingList(IList<T> list)
			: base(list)
		{
		}

		/// <summary>
		/// Overrides the OnListChanged method to ensure that modifications to the BindingList are performed on the main UI thread by using the captured synchronization context.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnListChanged(ListChangedEventArgs e)
		{
			if (_syncContext == null)
				base.OnListChanged(e);
			else
				_syncContext.Send(state => BaseListChanged(e), null);
		}
		private void BaseAddingNew(AddingNewEventArgs e)
		{
			base.OnAddingNew(e);
		}
		private void BaseListChanged(ListChangedEventArgs e)
		{
			base.OnListChanged(e);
		}
	}
}
