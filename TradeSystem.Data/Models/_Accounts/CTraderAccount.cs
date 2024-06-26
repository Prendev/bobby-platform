﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TradeSystem.Data.Models
{
    public class CTraderAccount : BaseDescriptionEntity
	{
        public long AccountNumber { get; set; }
		[Required] public string AccessToken { get; set; }

        public int CTraderPlatformId { get; set; }
        public CTraderPlatform CTraderPlatform { get; set; }

		public List<Account> Accounts { get; } = new List<Account>();

		public override string ToString()
        {
            return $"{(Id == 0 ? "UNSAVED - " : "")}{Description} ({AccountNumber})";
        }
    }
}
