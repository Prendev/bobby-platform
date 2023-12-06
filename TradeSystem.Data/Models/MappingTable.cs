using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public class MappingTable : BaseEntity
    {
        [InvisibleColumn] public int CustomGroupId { get; set; }
        [InvisibleColumn] public CustomGroup CustomGroup { get; set; }

        [Required]
        [DisplayPriority(0)]
        public string BrokerName { get; set; }

        [Required]
        [DisplayPriority(1)]
        public string Instrument { get; set; }

        [Required]
        [DisplayPriority(2)]
        public int LotSize { get; set; }

        public override string ToString()
        {
            return (Id == 0 ? "UNSAVED - " : "") + BrokerName;
        }
    }
}
