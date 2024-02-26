using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace TradeSystem.Duplicat.ViewModel
{
	public class SortableBindingList<T> : BindingList<T> where T : class
	{
		private bool _isSorted;
		private ListSortDirection _sortDirection = ListSortDirection.Ascending;
		private PropertyDescriptor _sortProperty;

		private readonly SynchronizationContext m_syncContext = SynchronizationContext.Current;

		/// <summary>
		/// Initializes a new instance of the <see cref="SortableBindingList{T}"/> class.
		/// </summary>
		public SortableBindingList()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SortableBindingList{T}"/> class.
		/// </summary>
		/// <param name="list">An <see cref="T:System.Collections.Generic.IList`1" /> of items to be contained in the <see cref="T:System.ComponentModel.BindingList`1" />.</param>
		public SortableBindingList(IList<T> list)
			: base(list)
		{
		}

		/// <summary>
		/// Gets a value indicating whether the list supports sorting.
		/// </summary>
		protected override bool SupportsSortingCore
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether the list is sorted.
		/// </summary>
		protected override bool IsSortedCore
		{
			get { return _isSorted; }
		}

		/// <summary>
		/// Gets the direction the list is sorted.
		/// </summary>
		protected override ListSortDirection SortDirectionCore
		{
			get { return _sortDirection; }
		}

		/// <summary>
		/// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null
		/// </summary>
		protected override PropertyDescriptor SortPropertyCore
		{
			get { return _sortProperty; }
		}

		/// <summary>
		/// Removes any sort applied with ApplySortCore if sorting is implemented
		/// </summary>
		protected override void RemoveSortCore()
		{
			_sortDirection = ListSortDirection.Ascending;
			_sortProperty = null;
			_isSorted = false; //thanks Luca
		}

		/// <summary>
		/// Sorts the items if overridden in a derived class
		/// </summary>
		/// <param name="prop"></param>
		/// <param name="direction"></param>
		protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
		{
			_sortProperty = prop;
			_sortDirection = direction;

			List<T> list = Items as List<T>;
			if (list == null) return;

			list.Sort(Compare);

			_isSorted = true;
			//fire an event that the list has been changed.
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		/// <summary>
		/// Overrides the OnAddingNew method to ensure that modifications to the BindingList are performed on the main UI thread by using the captured synchronization context.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnAddingNew(AddingNewEventArgs e)
		{
			if (m_syncContext == null)
				BaseAddingNew(e);
			else
				m_syncContext.Send(state => BaseAddingNew(e), null);
		}

		/// <summary>
		/// Overrides the OnListChanged method to ensure that modifications to the BindingList are performed on the main UI thread by using the captured synchronization context.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnListChanged(ListChangedEventArgs e)
		{
			if (m_syncContext == null)
				base.OnListChanged(e);
			else
				m_syncContext.Send(state => BaseListChanged(e), null);
		}
		private void BaseAddingNew(AddingNewEventArgs e)
		{
			base.OnAddingNew(e);
		}
		private void BaseListChanged(ListChangedEventArgs e)
		{
			base.OnListChanged(e);
		}


		private int Compare(T lhs, T rhs)
		{
			var result = OnComparison(lhs, rhs);
			//invert if descending
			if (_sortDirection == ListSortDirection.Descending)
				result = -result;
			return result;
		}

		private int OnComparison(T lhs, T rhs)
		{
			object lhsValue = lhs == null ? null : _sortProperty.GetValue(lhs);
			object rhsValue = rhs == null ? null : _sortProperty.GetValue(rhs);
			if (lhsValue == null)
			{
				return (rhsValue == null) ? 0 : -1; //nulls are equal
			}
			if (rhsValue == null)
			{
				return 1; //first has value, second doesn't
			}
			if (lhsValue is IComparable)
			{
				return ((IComparable)lhsValue).CompareTo(rhsValue);
			}
			if (lhsValue.Equals(rhsValue))
			{
				return 0; //both are the same
			}
			//not comparable, compare ToString
			return lhsValue.ToString().CompareTo(rhsValue.ToString());
		}
	}
}