using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class CTraderAccount : BaseDescriptionEntity
	{
        public long AccountNumber { get; set; }
		[Required] public string AccessToken { get; set; }

        public int CTraderPlatformId { get; set; }
        public CTraderPlatform CTraderPlatform { get; set; }

		public List<Account> Accounts { get => Get(() => new List<Account>()); set => Set(value); }

		public override string ToString()
        {
            return $"{(Id == 0 ? "UNSAVED - " : "")}{Description} ({AccountNumber})";
        }
    }
}
