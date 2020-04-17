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
		[Category("Base")]
		public int Id { get; set; }

		[NotMapped]
		[InvisibleColumn]
		[Category("Base")]
		public string DisplayMember => ToString();

		[ReadOnly(true)]
		[Category("Base")]
		public DateTime TimeCreated { get; set; } = HiResDatetime.UtcNow;

        public override string ToString()
        {
            return Id == 0 ? "UNSAVED - " : Id.ToString();
        }
    }
}
