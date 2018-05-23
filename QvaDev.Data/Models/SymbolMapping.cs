using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class SymbolMapping : BaseEntity, IFilterableEntity
    {
	    [InvisibleColumn] public int SlaveId { get; set; }
	    [InvisibleColumn] public Slave Slave { get; set; }

        public string From { get; set; }
        public string To { get; set; }

        [NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
    }
}
