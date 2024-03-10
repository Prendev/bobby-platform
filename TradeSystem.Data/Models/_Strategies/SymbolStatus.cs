using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using TradeSystem.Common;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class SymbolStatus : BaseNotifyPropertyChange
	{
		private CustomGroup customGroup;

		[ReadOnly(true)]
		[DisplayName("Group Name")]
		public string Symbol { get => Get<string>(); set => Set(value); }

		[InvisibleColumn]
		public bool IsCreatedGroup { get; set; }

		[CheckBox]
		public bool IsVisible { get => Get<bool>(); set => Set(value); }


		[InvisibleColumn]
		public ObservableCollection<AccountLot> AccountLotList { get; set; } = new ObservableCollection<AccountLot>();

		[InvisibleColumn]
		public CustomGroup CustomGroup
		{
			get { return customGroup; }
			set
			{
				customGroup = value;
				customGroup.PropertyChanged += Value_PropertyChanged;

				Symbol = customGroup.GroupName;
			}
		}

		public SymbolStatus()
		{
			AccountLotList.CollectionChanged += AccountSum_CollectionChanged;
		}

		private void AccountSum_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(AccountLotList));
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var item in e.NewItems)
				{
					(item as AccountLot).LotChanged += SymbolStatus_LotChanged;
				}
			}
		}

		private void SymbolStatus_LotChanged(object sender, decimal e)
		{
			OnPropertyChanged(nameof(AccountLot.SumLot));
		}

		private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Symbol = customGroup.GroupName;
		}
	}

	public class AccountLot : BaseNotifyPropertyChange
	{
		private decimal sumLot;
		public Account Account { get; set; }
        public string Instrument { get; set; }
        public decimal SumLot
		{
			get { return sumLot; }
			set
			{
				if (value != sumLot)
				{
					sumLot = value;
					LotChanged?.Invoke(this, value);
				}
			}
		}

		public event EventHandler<decimal> LotChanged;
	}
}
