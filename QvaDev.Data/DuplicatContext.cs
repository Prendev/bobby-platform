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

        public DbSet<Profile> Profiles { get; set; }
		public DbSet<Account> Accounts { get; set; }

		public DbSet<Master> Masters { get; set; }
        public DbSet<Slave> Slaves { get; set; }

        public DbSet<Copier> Copiers { get; set; }
	    public DbSet<FixApiCopier> FixApiCopiers { get; set; }
		public DbSet<SymbolMapping> SymbolMappings { get; set; }

        public DbSet<Monitor> Monitors { get; set; }
        public DbSet<MonitoredAccount> MonitoredAccounts { get; set; }
        public DbSet<Pushing> Pushings { get; set; }
        public DbSet<PushingDetail> PushingDetails { get; set; }

        public DbSet<Expert> Experts { get; set; }
        public DbSet<TradingAccount> TradingAccounts { get; set; }
        public DbSet<QuadroSet> QuadroSets { get; set; }

		public DbSet<Ticker> Tickers { get; set; }

	    public DbSet<StratDealingArb> StratDealingArbs { get; set; }

		public void Init()
		{
			var exists = Database.Exists();
			if (!exists) Database.Create();

			try
			{
				foreach (var srv in Directory.GetFiles(".\\Mt4SrvFiles", "*.srv").Select(Path.GetFileNameWithoutExtension))
				{
					if (MetaTraderPlatforms.Any(p => p.Description == srv)) continue;
					MetaTraderPlatforms.Add(new MetaTraderPlatform()
					{
						Description = srv,
						SrvFilePath = $"Mt4SrvFiles\\{srv}.srv"
					});
				}

				SaveChanges();
			}
			catch { }
		}
	}
}
