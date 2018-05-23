using System.Collections.Generic;

namespace QvaDev.Data.Models
{
    public class Profile : BaseDescriptionEntity
    {
        public List<Monitor> Monitors { get => Get(() => new List<Monitor>()); set => Set(value, false); }
        public List<TradingAccount> TradingAccounts { get => Get(() => new List<TradingAccount>()); set => Set(value, false); }
        public List<Pushing> Pushings { get => Get(() => new List<Pushing>()); set => Set(value, false); }
    }
}
