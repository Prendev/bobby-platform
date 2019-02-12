using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradeSystem.Data
{
	public static class Extensions
	{
		public static PropertyBuilder<decimal?> HasPrecision(this PropertyBuilder<decimal?> builder, int precision, int scale)
		{
			return builder.HasColumnType($"decimal({precision},{scale})");
		}

		public static PropertyBuilder<decimal> HasPrecision(this PropertyBuilder<decimal> builder, int precision, int scale)
		{
			return builder.HasColumnType($"decimal({precision},{scale})");
		}
	}
}
