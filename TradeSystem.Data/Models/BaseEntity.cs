using System;
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
		public int Id { get; set; }

		[InvisibleColumn]
		public DateTime? CreatedAt { get; set; }

		[InvisibleColumn]
		public DateTime? UpdatedAt { get; set; }

		[NotMapped]
		[InvisibleColumn]
		public string DisplayMember => ToString();

		public override string ToString()
		{
			return Id == 0 ? "UNSAVED - " : Id.ToString();
		}
	}
}
