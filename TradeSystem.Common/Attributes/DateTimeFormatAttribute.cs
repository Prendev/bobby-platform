using System;

namespace TradeSystem.Common.Attributes
{
    public class DateTimeFormatAttribute : Attribute
    {
        public string DateTimeFormat { get; }

        public DateTimeFormatAttribute(string dateTimeFormat)
        {
			DateTimeFormat = dateTimeFormat;
        }
    }
}
