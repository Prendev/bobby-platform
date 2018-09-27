using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QvaDev.Data.Models;
using QvaDev.FileContextCore.Extensions;

namespace QvaDev.Data
{
    public class DuplicatContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionString = ConfigurationManager.ConnectionStrings["DuplicatContext"];
			var cs = connectionString.ConnectionString;

			var dd = AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString();
			if (!string.IsNullOrWhiteSpace(dd) && cs.Contains("|DataDirectory|"))
				cs = cs.Replace("|DataDirectory|", dd);

			if (connectionString.ProviderName.ToLowerInvariant() == "filecontext")
				optionsBuilder.UseFileContext(databasename: cs);
			else if (connectionString.ProviderName.ToLowerInvariant() == "mssql")
				optionsBuilder.UseSqlServer(cs);
			else if (connectionString.ProviderName.ToLowerInvariant() == "sqlite")
				optionsBuilder.UseSqlite(cs);
			else throw new ArgumentException("ProviderName");
		}

		public DbSet<MetaTraderPlatform> MetaTraderPlatforms { get; set; }
        public DbSet<CTraderPlatform> CTraderPlatforms { get; set; }
        public DbSet<MetaTraderAccount> MetaTraderAccounts { get; set; }
        public DbSet<CTraderAccount> CTraderAccounts { get; set; }
        public DbSet<FixTraderAccount> FixTraderAccounts { get; set; }
		public DbSet<FixApiAccount> FixApiAccounts { get; set; }
		public DbSet<IlyaFastFeedAccount> IlyaFastFeedAccounts { get; set; }
		public DbSet<CqgClientApiAccount> CqgClientApiAccounts { get; set; }

		public DbSet<Profile> Profiles { get; set; }
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Aggregator> Aggregators { get; set; }
		public DbSet<AggregatorAccount> AggregatorAccounts { get; set; }

		public DbSet<Master> Masters { get; set; }
        public DbSet<Slave> Slaves { get; set; }

        public DbSet<Copier> Copiers { get; set; }
	    public DbSet<FixApiCopier> FixApiCopiers { get; set; }
		public DbSet<SymbolMapping> SymbolMappings { get; set; }

        public DbSet<Pushing> Pushings { get; set; }
        public DbSet<PushingDetail> PushingDetails { get; set; }

		public DbSet<Ticker> Tickers { get; set; }

	    public DbSet<StratDealingArb> StratDealingArbs { get; set; }
	    public DbSet<StratDealingArbPosition> StratDealingArbPositions { get; set; }

		public DbSet<StratPosition> Positions { get; set; }
		public DbSet<StratHubArb> StratHubArbs { get; set; }
		public DbSet<StratHubArbPosition> StratHubArbPositions { get; set; }

		public void Init()
		{
			if (bool.TryParse(ConfigurationManager.AppSettings["AllowDatabaseMigration"], out bool migrate) && migrate)
				Database.Migrate();

			try
			{
				foreach (var name in Directory.GetFiles(".\\Mt4SrvFiles", "*.srv").Select(Path.GetFileNameWithoutExtension))
				{
					if (MetaTraderPlatforms.Any(p => p.Description == name)) continue;
					MetaTraderPlatforms.Add(new MetaTraderPlatform()
					{
						Description = name,
						SrvFilePath = $"Mt4SrvFiles\\{name}.srv"
					});
				}

				SaveChanges();
			}
			catch { }

			try
			{
				foreach (var name in Directory.GetFiles(".\\FixApiConfigFiles", "*.xml").Select(Path.GetFileNameWithoutExtension))
				{
					if (FixApiAccounts.Any(p => p.Description == name)) continue;
					FixApiAccounts.Add(new FixApiAccount()
					{
						Description = name,
						ConfigPath = $"FixApiConfigFiles\\{name}.xml"
					});
				}

				SaveChanges();
			}
			catch { }
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<StratDealingArb>().Property(x => x.PipSize).HasPrecision(18, 5);
			modelBuilder.Entity<StratDealingArb>().Property(x => x.AlphaSize).HasPrecision(18, 3);
			modelBuilder.Entity<StratDealingArb>().Property(x => x.BetaSize).HasPrecision(18, 3);
			modelBuilder.Entity<StratDealingArb>().Property(x => x.SlippageInPip).HasPrecision(18, 2);

			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.AlphaOpenPrice).HasPrecision(18, 5);
			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.BetaOpenPrice).HasPrecision(18, 5);
			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.AlphaClosePrice).HasPrecision(18, 5);
			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.BetaClosePrice).HasPrecision(18, 5);
			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.AlphaOpenSignal).HasPrecision(18, 5);
			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.BetaOpenSignal).HasPrecision(18, 5);
			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.AlphaCloseSignal).HasPrecision(18, 5);
			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.BetaCloseSignal).HasPrecision(18, 5);

			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.AlphaSize).HasPrecision(18, 3);
			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.BetaSize).HasPrecision(18, 3);
			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.RemainingAlpha).HasPrecision(18, 3);
			modelBuilder.Entity<StratDealingArbPosition>().Property(x => x.RemainingBeta).HasPrecision(18, 3);

			modelBuilder.Entity<StratHubArb>().Property(x => x.PipSize).HasPrecision(18, 5);
			modelBuilder.Entity<StratHubArb>().Property(x => x.Size).HasPrecision(18, 3);
			modelBuilder.Entity<StratHubArb>().Property(x => x.SlippageInPip).HasPrecision(18, 2);

			modelBuilder.Entity<FixApiCopier>().Property(x => x.PipSize).HasPrecision(18, 5);
			modelBuilder.Entity<FixApiCopier>().Property(x => x.SlippageInPip).HasPrecision(18, 2);

			modelBuilder.Entity<AggregatorAccount>().HasKey(c => new { c.AggregatorId, c.AccountId });

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
