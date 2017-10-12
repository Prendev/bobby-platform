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

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Master> Masters { get; set; }
        public DbSet<Slave> Slaves { get; set; }

        public DbSet<Copier> Copiers { get; set; }
        public DbSet<SymbolMapping> SymbolMappings { get; set; }

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
            }
            catch  { }

            if (exists) return;

            CTraderPlatforms.Add(new CTraderPlatform()
            {
                Description = "cTrader Prod",
                AccountsApi = "https://api.spotware.com",
                TradingHost = "tradeapi.spotware.com",
                AccessToken = "802TOtADXELJqQ9gkiYfCLbqWWMqfbLWzzMPtg9PUa8",
                ClientId = "353_RTrdmK0Q01Czcaq1p19Y4RbJgngCn7Wc065c7QnFqHuUfr127S",
                Secret = "tQjMpGEbdvP68VXzm9sCQY3tuawrApSl5ZvmOGCAqov1eFT3zn",
                Playground = "https://connect.spotware.com/apps/353/playground",
                AccessBaseUrl = "https://connect.spotware.com/apps"
            });
            CTraderPlatforms.Add(new CTraderPlatform()
            {
                Description = "cTrader Sandbox",
                AccountsApi = "https://sandbox-api.spotware.com",
                TradingHost = "sandbox-tradeapi.spotware.com",
                AccessToken = "?",
                ClientId = "?",
                Secret = "?",
                Playground = "?",
                AccessBaseUrl = "https://sandbox-connect.spotware.com/apps"
            });

            SaveChanges();

            var mt4Platform = MetaTraderPlatforms.First(p => p.Description == "AuraFX-Demo");
            var cTraderPlatform = CTraderPlatforms.First(p => p.Description == "cTrader Prod");

            MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Aura1",
                User = 8801562,
                Password = "k0agjjf",
                MetaTraderPlatform = mt4Platform
            });
            MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Aura2",
                User = 8801567,
                Password = "4kfmvmo",
                MetaTraderPlatform = mt4Platform
            });

            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader1",
                AccountNumber = 3174636,
                CTraderPlatform = cTraderPlatform
            });
            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader2",
                AccountNumber = 3174645,
                CTraderPlatform = cTraderPlatform
            });
            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader3",
                AccountNumber = 3175387,
                CTraderPlatform = cTraderPlatform
            });
            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader4",
                AccountNumber = 3175388,
                CTraderPlatform = cTraderPlatform
            });

            Profiles.Add(new Profile() { Description = "Dummy Profile 1" });
            Profiles.Add(new Profile() { Description = "Dummy Profile 2" });

            SaveChanges();

            Groups.Add(new Group() {Description = "Dummy Group 1", Profile = Profiles.First(p => p.Description == "Dummy Profile 1") });
            Groups.Add(new Group() {Description = "Dummy Group 2", Profile = Profiles.First(p => p.Description == "Dummy Profile 1") });
            Groups.Add(new Group() {Description = "Dummy Group 3", Profile = Profiles.First(p => p.Description == "Dummy Profile 2") });
            Groups.Add(new Group() {Description = "Dummy Group 4", Profile = Profiles.First(p => p.Description == "Dummy Profile 2") });

            SaveChanges();

            Masters.Add(new Master
            {
                Group = Groups.First(a => a.Description == "Dummy Group 1"),
                MetaTraderAccount = MetaTraderAccounts.First(a => a.Description == "Aura1")
            });
            Masters.Add(new Master
            {
                Group = Groups.First(a => a.Description == "Dummy Group 2"),
                MetaTraderAccount = MetaTraderAccounts.First(a => a.Description == "Aura2")
            });

            SaveChanges();

            Slaves.Add(new Slave
            {
                Master = Masters.ToList().First(),
                CTraderAccount = CTraderAccounts.First(a => a.Description == "cTrader1")
            });
            Slaves.Add(new Slave
            {
                Master = Masters.ToList().First(),
                CTraderAccount = CTraderAccounts.First(a => a.Description == "cTrader2")
            });
            Slaves.Add(new Slave
            {
                Master = Masters.ToList().Last(),
                CTraderAccount = CTraderAccounts.First(a => a.Description == "cTrader3")
            });
            Slaves.Add(new Slave
            {
                Master = Masters.ToList().Last(),
                CTraderAccount = CTraderAccounts.First(a => a.Description == "cTrader4")
            });

            SaveChanges();

            var slave = Slaves.First(x => x.Id == 1);
            SymbolMappings.Add(new SymbolMapping {Slave = slave, From = "GER30.pri", To = "GERMAN30"});
            SymbolMappings.Add(new SymbolMapping {Slave = slave, From = "UK100.pri", To = "UK100"});
            SymbolMappings.Add(new SymbolMapping {Slave = slave, From = "US30.pri", To = "US30.PRM"});
            SymbolMappings.Add(new SymbolMapping {Slave = slave, From = "XAUUSD.pri", To = "XAUUSD" });
            SymbolMappings.Add(new SymbolMapping {Slave = slave, From = "EURUSD+", To = "EURUSD" });

            slave = Slaves.First(x => x.Id == 2);
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "GER30.ex", To = "GERMAN30" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "UK100.ex", To = "UK100" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "US30.ex", To = "US30.PRM" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "XAUUSD.ex", To = "XAUUSD" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "EURUSD+", To = "EURUSD" });

            slave = Slaves.First(x => x.Id == 3);
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "GER30.ex", To = "GERMAN30" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "UK100.ex", To = "UK100" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "US30.ex", To = "US30.PRM" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "XAUUSD.ex", To = "XAUUSD" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "EURUSD+", To = "EURUSD" });

            slave = Slaves.First(x => x.Id == 4);
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "GER30.ex", To = "GERMAN30" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "UK100.ex", To = "UK100" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "US30.ex", To = "US30.PRM" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "XAUUSD.ex", To = "XAUUSD" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave, From = "EURUSD+", To = "EURUSD" });

            Copiers.Add(new Copier { Slave = Slaves.First(x => x.Id == 1), CopyRatio = 1, UseMarketRangeOrder = true,
                SlippageInPips = 30, MaxRetryCount = 5, RetryPeriodInMilliseconds = 3000 });
            Copiers.Add(new Copier { Slave = Slaves.First(x => x.Id == 2), CopyRatio = 1, UseMarketRangeOrder = true,
                SlippageInPips = 30, MaxRetryCount = 5, RetryPeriodInMilliseconds = 3000 });
            Copiers.Add(new Copier { Slave = Slaves.First(x => x.Id == 3), CopyRatio = 1, UseMarketRangeOrder = true,
                SlippageInPips = 30, MaxRetryCount = 5, RetryPeriodInMilliseconds = 3000 });
            Copiers.Add(new Copier { Slave = Slaves.First(x => x.Id == 4), CopyRatio = 1, UseMarketRangeOrder = true,
                SlippageInPips = 30, MaxRetryCount = 5, RetryPeriodInMilliseconds = 3000 });

            SaveChanges();
        }
    }
}
