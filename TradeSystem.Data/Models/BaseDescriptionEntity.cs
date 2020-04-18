using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public abstract class BaseDescriptionEntity : BaseEntity
	{
		private const string BaseCategory = "0 - Altalanos";

		[Required]
        [DisplayPriority(0)]
        [Category(BaseCategory)]
        [DisplayName("Megnevezes")]
		public string Description { get; set; }

        public override string ToString()
        {
            return (Id == 0 ? "UNSAVED - " : "") + Description;
        }
    }
}
