using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public class CustomGroup : BaseEntity
    {
        public List<MappingTable> MappingTables { get; } = new List<MappingTable>();

        [Required]
        [DisplayPriority(0)]
        public string GroupName { get; set; }

        public override string ToString()
        {
            return (Id == 0 ? "UNSAVED - " : "") + GroupName;
        }
    }
}
