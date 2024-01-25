using System;

namespace TradeSystem.Common.Attributes
{
    public class DateTimePickerAttribute : Attribute
    {
        public string Format { get; }

		public DateTimePickerAttribute(string format = "yyyy-MM-dd HH:mm:ss")
		{
			Format = format;
		}
	}
}
