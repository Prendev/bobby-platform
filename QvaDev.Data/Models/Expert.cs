using System.Collections.Generic;

namespace QvaDev.Data.Models
{
    public class Expert : BaseDescriptionEntity
    {
        public List<TradingAccount> TradingAccounts { get => Get(() => new List<TradingAccount>()); set => Set(value, false); }
    }
}
