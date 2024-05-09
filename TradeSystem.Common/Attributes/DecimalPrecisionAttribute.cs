using System;

namespace TradeSystem.Common.Attributes
{
    public class DecimalPrecisionAttribute : Attribute
    {
        public int DecimalPlaces { get; }

		public DecimalPrecisionAttribute(int decimalPlaces = -1)
        {
			DecimalPlaces = decimalPlaces;
        }
    }
}
