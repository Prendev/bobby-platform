using System.Data.Entity;
using System.IO;
using System.Linq;
using QvaDev.Data.Models;

namespace QvaDev.Data
{
    public class DuplicatContext : DbContext
	{
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
	    public DbSet<StratHubArb> StratHubArbs { get; set; }

	    //public DbSet<Position> Positions { get; set; }
	    //public DbSet<StratHubArbPosition> StratHubArbPositions { get; set; }

		public void Init()
		{
			var exists = Database.Exists();
			if (!exists) Database.Create();

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

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
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
		}
	}
}
