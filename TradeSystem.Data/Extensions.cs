using System.Collections.Generic;
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

		public static void AddSafe<T>(this List<T> list, T item)
		{
			if (list == null) return;
			lock (list) list.Add(item);
		}
	}
}
