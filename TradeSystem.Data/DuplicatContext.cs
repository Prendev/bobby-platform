using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TradeSystem.Data.Models;
using TradeSystem.FileContextCore.Extensions;

namespace TradeSystem.Data
{
    public class DuplicatContext : DbContext
    {
	    private readonly string _connectionString;
	    private readonly string _baseDirectory;

	    public DuplicatContext()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["DuplicatContext"];

			_connectionString = connectionString.ConnectionString;

			var dd = AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString();
			if (!string.IsNullOrWhiteSpace(dd) && _connectionString.Contains("|DataDirectory|"))
				_connectionString = _connectionString.Replace("|DataDirectory|", dd);

			_baseDirectory = AppContext.BaseDirectory;
		}

		public DuplicatContext(string connectionString, string providerName, string baseDirectory)
	    {
		    _connectionString = connectionString;
		    _baseDirectory = baseDirectory;
	    }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseFileContext(_connectionString, _baseDirectory);
		}

		public DbSet<Profile> Profiles { get; set; }
		public DbSet<Quotation> Quotations { get; set; }

		public void Init()
		{
			if (bool.TryParse(ConfigurationManager.AppSettings["AllowDatabaseMigration"], out bool migrate) && migrate)
				Database.Migrate();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//modelBuilder.Entity<AggregatorAccount>().HasKey(x => new { x.AggregatorId, x.AccountId });
			//modelBuilder.Entity<AggregatorAccount>()
			//	.HasOne(x => x.Aggregator).WithMany(x => x.Accounts)
			//	.HasForeignKey(x => x.AggregatorId);
			//modelBuilder.Entity<AggregatorAccount>()
			//	.HasOne(x => x.Account).WithMany(x => x.Aggregators)
			//	.HasForeignKey(x => x.AccountId);

			var timeSpanConverter = new ValueConverter<TimeSpan, long>(v => v.Ticks, v => new TimeSpan(v));
			var nullTimeSpanConverter = new ValueConverter<TimeSpan?, long?>(v => v != null ? v.Value.Ticks : (long?) null,
				v => v != null ? new TimeSpan(v.Value) : (TimeSpan?) null);

			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			foreach (var property in entityType.GetProperties())
			{
				if (property.ClrType == typeof(TimeSpan) || property.ClrType == typeof(TimeSpan?))
					property.SetValueConverter(timeSpanConverter);
				else if (property.ClrType == typeof(TimeSpan?))
					property.SetValueConverter(nullTimeSpanConverter);
			}
		}
	}
}
