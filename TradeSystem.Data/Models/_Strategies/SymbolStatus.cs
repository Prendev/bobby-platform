using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Security.Policy;
using TradeSystem.Common;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	public class SymbolStatus : BaseNotifyPropertyChange
	{
		[ReadOnly(true)]
		[DisplayName("Group Name")]
		public string Symbol { get; set; }

		[InvisibleColumn]
		public bool IsCreatedGroup { get; set; }

		[CheckBox]
		public bool IsVisible { get => Get<bool>(); set => Set(value); }


		[InvisibleColumn]
		public ObservableCollection<AccountLot> AccountSum { get; set; } = new ObservableCollection<AccountLot>();

		public SymbolStatus()
		{
			AccountSum.CollectionChanged += AccountSum_CollectionChanged;
		}

		private void AccountSum_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(AccountSum));
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

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			SymbolStatus other = (SymbolStatus)obj;
			return Symbol == other.Symbol && IsCreatedGroup == other.IsCreatedGroup;
		}

		public override int GetHashCode()
		{
			return Symbol.GetHashCode() + IsCreatedGroup.GetHashCode();
		}
	}

	public class AccountLot : BaseNotifyPropertyChange
	{
		private decimal sumLot;
		public Account Account { get; set; }

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
