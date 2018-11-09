using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class SymbolMapping : BaseEntity
	{
		public string From { get; set; }
		public string To { get; set; }

		[InvisibleColumn] public int SlaveId { get; set; }
	    [InvisibleColumn] public Slave Slave { get; set; }
    }
}
