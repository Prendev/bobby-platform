using System;

namespace TradeSystem.Common.Attributes
{
    public class DateTimePickerAttribute : Attribute
    {
        public string Format { get; }

		public DateTimePickerAttribute(string format = null)
		{
			Format = format;
		}
	}
}
