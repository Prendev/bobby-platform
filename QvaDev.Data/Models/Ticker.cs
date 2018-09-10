﻿using QvaDev.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class Ticker : BaseEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public int MainAccountId { get; set; }
		public Account MainAccount { get; set; }
		public string MainSymbol { get; set; }

		public int? PairAccountId { get; set; }
        public Account PairAccount { get; set; }
		public string PairSymbol { get; set; }
	}
}