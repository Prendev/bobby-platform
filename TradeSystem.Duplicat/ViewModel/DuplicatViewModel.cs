using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using TradeSystem.Common;
using TradeSystem.Data;
using TradeSystem.Data.Models;
using TradeSystem.Orchestration;
using IBindingList = System.ComponentModel.IBindingList;

namespace TradeSystem.Duplicat.ViewModel
{
	public partial class DuplicatViewModel : BaseNotifyPropertyChange
	{
		public enum SaveStates
		{
			Default,
			Error,
			Success
		}

		public delegate void DataContextChangedEventHandler();

		private DuplicatContext _duplicatContext;
		private readonly IOrchestrator _orchestrator;
		private readonly List<PropertyChangedEventHandler> _propertyChangedDelegates = new List<PropertyChangedEventHandler>();
		private readonly List<Tuple<IBindingList, ListChangedEventHandler>> _listChangedDelegates =
			new List<Tuple<IBindingList, ListChangedEventHandler>>();

		public event DataContextChangedEventHandler DataContextChanged;

		public BindingList<Profile> Profiles { get; private set; }
		public BindingList<Quotation> Quotations { get; private set; }
		
		public bool IsLoading { get => Get<bool>(); set => Set(value); }
		public bool IsConnected { get => Get<bool>(); set => Set(value); }
		public SaveStates SaveState { get => Get<SaveStates>(); set => Set(value); }

		public Profile SelectedProfile { get => Get<Profile>(); set => Set(value); }

		public DuplicatViewModel(IOrchestrator orchestrator)
		{
			_orchestrator = orchestrator;
			InitDataContext();
		}

		private void InitDataContext()
		{
			_duplicatContext?.Dispose();
			_duplicatContext = new DuplicatContext();
			LoadLocals();
			_orchestrator.Init(_duplicatContext);
			DataContextChanged?.Invoke();
		}

		private void LoadLocals()
		{
			foreach (var propertyChangedEventHandler in _propertyChangedDelegates) PropertyChanged -= propertyChangedEventHandler;
			_propertyChangedDelegates.Clear();
			foreach (var listChanged in _listChangedDelegates) listChanged.Item1.ListChanged -= listChanged.Item2;
			_listChangedDelegates.Clear();

			_duplicatContext.Profiles.OrderBy(e => e.ToString()).Load();
			Profiles = _duplicatContext.Profiles.Local.ToBindingList();

			_duplicatContext.Quotations.OrderByDescending(e => e.Id).Load();
			Quotations = _duplicatContext.Quotations.Local.ToBindingList();
		}

		private BindingList<T> ToBindingList<T, TSelected>(
			ICollection<T> local,
			Expression<Func<TSelected>> selected,
			Func<TSelected, List<T>> property) where T : class where TSelected : class
		{
			var bindingList = new BindingList<T>();
			var items = new List<T>();
			var sync = true;

			void ListChanged(object sender, ListChangedEventArgs args)
			{
				if (!sync) return;

				if (args.ListChangedType == ListChangedType.ItemAdded)
				{
					items.Add(bindingList[args.NewIndex]);
					local.Add(bindingList[args.NewIndex]);

					var sel = selected.Compile().Invoke();
					if (sel == null) return;
					var list = property.Invoke(selected.Compile().Invoke());
					list.Add(bindingList[args.NewIndex]);
				}

				if (args.ListChangedType == ListChangedType.ItemDeleted)
				{
					local.Remove(items[args.NewIndex]);

					var sel = selected.Compile().Invoke();
					if (sel != null)
					{
						var list = property.Invoke(selected.Compile().Invoke());
						list.Remove(items[args.NewIndex]);
					}

					items.RemoveAt(args.NewIndex);
				}
			}

			_listChangedDelegates.Add(new Tuple<IBindingList, ListChangedEventHandler>(bindingList, ListChanged));
			bindingList.ListChanged += ListChanged;

			void PropChanged(object sender, PropertyChangedEventArgs args)
			{
				var sn = ((MemberExpression)selected.Body).Member.Name;
				if (args.PropertyName != sn) return;
				sync = false;
				bindingList.Clear();
				items.Clear();

				var sel = selected.Compile().Invoke();
				if (sel != null)
				{
					var list = property.Invoke(selected.Compile().Invoke());
					foreach (var e in list)
					{
						items.Add(e);
						bindingList.Add(e);
					}
				}
				
				sync = true;
			}

			_propertyChangedDelegates.Add(PropChanged);
			PropertyChanged += PropChanged;
			return bindingList;
		}

		private BindingList<T> ToFilteredBindingList<T, TProp>(
			ICollection<T> local,
			Func<T, TProp> property,
			Expression<Func<TProp>> selected) where T : class where TProp : class
		{
			var bindingList = new BindingList<T>();
			var items = new List<T>();
			var sync = true;

			void ListChanged(object sender, ListChangedEventArgs args)
			{
				if (!sync) return;

				if (args.ListChangedType == ListChangedType.ItemAdded)
				{
					items.Add(bindingList[args.NewIndex]);
					local.Add(bindingList[args.NewIndex]);
				}

				if (args.ListChangedType == ListChangedType.ItemDeleted)
				{
					local.Remove(items[args.NewIndex]);
					items.RemoveAt(args.NewIndex);
				}
			}

			_listChangedDelegates.Add(new Tuple<IBindingList, ListChangedEventHandler>(bindingList, ListChanged));
			bindingList.ListChanged += ListChanged;

			void PropChanged(object sender, PropertyChangedEventArgs args)
			{
				var sn = ((MemberExpression)selected.Body).Member.Name;
				if (args.PropertyName != sn) return;
				sync = false;
				bindingList.Clear();
				items.Clear();
				var sel = selected.Compile().Invoke();
				foreach (var e in local.ToList().Where(e => property.Invoke(e) == sel))
				{
					items.Add(e);
					bindingList.Add(e);
				}
				sync = true;
			}

			_propertyChangedDelegates.Add(PropChanged);
			PropertyChanged += PropChanged;
			return bindingList;
		}
	}
}
