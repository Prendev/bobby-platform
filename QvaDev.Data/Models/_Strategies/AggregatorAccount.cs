﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public class AggregatorAccount : BaseNotifyPropertyChange, IFilterableEntity
	{
		[InvisibleColumn] [Key] public int AggregatorId { get; set; }
		[InvisibleColumn] public Aggregator Aggregator { get; set; }

		[Key] public int AccountId { get; set; }
		public Account Account { get; set; }

		[Required] public string Symbol { get; set; }

		[NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
	}
}
