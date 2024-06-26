﻿using System.Collections.Generic;

namespace TradeSystem.Data.Models
{
    public class Profile : BaseDescriptionEntity
    {
        public List<Pushing> Pushings { get; } = new List<Pushing>();
		public List<Account> Accounts { get; } = new List<Account>();
		public List<Aggregator> Aggregators { get; } = new List<Aggregator>();
	    public List<ProfileProxy> ProfileProxies { get; } = new List<ProfileProxy>();
		public List<Master> Masters { get; } = new List<Master>();
	}
}
