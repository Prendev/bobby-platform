using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
    public class CustomGroup : BaseEntity
    {
        public List<MappingTable> MappingTables { get; } = new List<MappingTable>();

        public string GroupName { get => Get<string>(); set => Set(value); }

		public override string ToString()
        {
            return (Id == 0 ? "UNSAVED - " : "") + GroupName;
        }
    }
}
