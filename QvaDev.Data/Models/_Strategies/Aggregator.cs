using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;
using QvaDev.Communication.FixApi;

namespace QvaDev.Data.Models
{
	public class Aggregator : BaseDescriptionEntity
	{
		public event EventHandler<GroupQuoteEventArgs> GroupQuote;

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		[InvisibleColumn] public List<AggregatorAccount> Accounts { get => Get(() => new List<AggregatorAccount>()); set => Set(value, false); }

		private IQuoteAggregator _quoteAggregator;
		[NotMapped]
		[InvisibleColumn]
		public IQuoteAggregator QuoteAggregator
		{
			get => _quoteAggregator;
			set
			{
				if (_quoteAggregator != null)
					_quoteAggregator.GroupQuote -= QuoteAggregator_GroupQuote;

				if (value != null)
					value.GroupQuote += QuoteAggregator_GroupQuote;

				_quoteAggregator = value;
			}
		}

		private void QuoteAggregator_GroupQuote(object sender, GroupQuoteEventArgs e)
		{
			GroupQuote?.Invoke(this, e);
		}
	}
}
