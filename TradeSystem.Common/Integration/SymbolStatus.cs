using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Common.Integration
{
    public class SymbolStatus : BaseNotifyPropertyChange
    {
        [ReadOnly(true)]
        [DisplayName("Group Name")]
        public string Symbol { get; set; }

        [InvisibleColumn]
        public bool IsCreatedGroup { get; set; }

        [DisplayName("Visible")]
        public bool IsVisible { get => Get<bool>(); set => Set(value); }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            SymbolStatus other = (SymbolStatus)obj;
            return Symbol == other.Symbol && IsCreatedGroup == other.IsCreatedGroup;
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode() + IsCreatedGroup.GetHashCode();
        }
    }
}
