using System;

namespace TradeSystem.Common.Attributes
{
	public class TooltipAttribute : Attribute
	{
		public string Tooltip { get; }

		public TooltipAttribute(string tooltip)
		{
			Tooltip = tooltip;
		}
	}
}
