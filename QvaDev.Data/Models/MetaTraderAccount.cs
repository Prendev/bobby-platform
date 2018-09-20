using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class MetaTraderAccount : BaseDescriptionEntity
	{
        public long User { get; set; }
        [Required] public string Password { get; set; }
        public int MetaTraderPlatformId { get; set; }
        public MetaTraderPlatform MetaTraderPlatform { get; set; }


		//public int RohadjaLe { get; set; }

		public List<Account> Accounts { get => Get(() => new List<Account>()); set => Set(value, false); }

		public override string ToString()
        {
            return $"{(Id == 0 ? "UNSAVED - " : "")}{Description} ({User})";
        }
    }
}
