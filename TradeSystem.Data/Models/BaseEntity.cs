using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public abstract class BaseEntity : BaseNotifyPropertyChange
	{
        [Key]
        [Dapper.Contrib.Extensions.Key]
        [InvisibleColumn]
        [ReadOnly(true)]
		[Category("Altalanos")]
        [DisplayName("ID")]
		public int Id { get; set; }

		[NotMapped]
		[InvisibleColumn]
		[Category("Altalanos")]
		[DisplayName("Megjelenitesi nev")]
		public string DisplayMember => ToString();

		[ReadOnly(true)]
		[Category("Altalanos")]
		[DisplayName("Letrehozva")]
		public DateTime TimeCreated { get; set; } = HiResDatetime.UtcNow;

        public override string ToString()
        {
            return Id == 0 ? "UNSAVED - " : Id.ToString();
        }
    }
}
