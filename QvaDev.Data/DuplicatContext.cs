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

        public DbSet<Monitor> Monitors { get; set; }
        public DbSet<MonitoredAccount> MonitoredAccounts { get; set; }

        public DbSet<Expert> Experts { get; set; }
        public DbSet<TradingAccount> TradingAccounts { get; set; }
        public DbSet<ExpertSet> ExpertSets { get; set; }

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

            Experts.Add(new Expert { Description = "Quadro" });

            SaveChanges();

            InitQuadro();
            SaveChanges();

            CTraderPlatforms.Add(new CTraderPlatform()
            {
                Description = "cTrader Prod",
                AccountsApi = "https://api.spotware.com",
                TradingHost = "tradeapi.spotware.com",
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
                User = 8801894,
                Password = "s2shqpj",
                MetaTraderPlatform = mt4Platform,
                ShouldConnect = true
            });
            MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Aura2",
                User = 8801895,
                Password = "v1iaxwc",
                MetaTraderPlatform = mt4Platform
            });

            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader1",
                AccountNumber = 3174636,
                CTraderPlatform = cTraderPlatform,
                AccessToken = "sVP14Z83qW455RoXW_c2dDvoF4PiIn-5-XxFVd2kwjs"
            });
            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader2",
                AccountNumber = 3174645,
                CTraderPlatform = cTraderPlatform,
                AccessToken = "sVP14Z83qW455RoXW_c2dDvoF4PiIn-5-XxFVd2kwjs"
            });
            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader3",
                AccountNumber = 3175387,
                CTraderPlatform = cTraderPlatform,
                AccessToken = "sVP14Z83qW455RoXW_c2dDvoF4PiIn-5-XxFVd2kwjs"
            });
            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader4",
                AccountNumber = 3175388,
                CTraderPlatform = cTraderPlatform,
                AccessToken = "sVP14Z83qW455RoXW_c2dDvoF4PiIn-5-XxFVd2kwjs"
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

            Monitors.Add(new Monitor
            {
                Description = "Side A",
                Profile = Profiles.First(p => p.Description == "Dummy Profile 1"),
                Symbol = "EURUSD"
            });
            Monitors.Add(new Monitor
            {
                Description = "Side B",
                Profile = Profiles.First(p => p.Description == "Dummy Profile 1"),
                Symbol = "EURUSD"
            });
            Monitors.Add(new Monitor
            {
                Description = "Side C",
                Profile = Profiles.First(p => p.Description == "Dummy Profile 1"),
                Symbol = "EURUSD"
            });
            Monitors.Add(new Monitor
            {
                Description = "Side D",
                Profile = Profiles.First(p => p.Description == "Dummy Profile 1"),
                Symbol = "EURUSD"
            });

            SaveChanges();

            MonitoredAccounts.Add(new MonitoredAccount
            {
                CTraderAccount = CTraderAccounts.First(e => e.Id == 1),
                Monitor = Monitors.First(e => e.Id == 1),
                ExpectedContracts = 1000
            });
            MonitoredAccounts.Add(new MonitoredAccount
            {
                CTraderAccount = CTraderAccounts.First(e => e.Id == 2),
                Monitor = Monitors.First(e => e.Id == 1),
                ExpectedContracts = 2000
            });

            MonitoredAccounts.Add(new MonitoredAccount
            {
                MetaTraderAccount = MetaTraderAccounts.First(e => e.Id == 1),
                Monitor = Monitors.First(e => e.Id == 1),
                ExpectedContracts = 1000,
                Symbol = "EURUSD+",
                IsMaster = true
            });
            MonitoredAccounts.Add(new MonitoredAccount
            {
                MetaTraderAccount = MetaTraderAccounts.First(e => e.Id == 2),
                Monitor = Monitors.First(e => e.Id == 2),
                ExpectedContracts = 1000,
                Symbol = "EURUSD+",
                IsMaster = true
            });

            MonitoredAccounts.Add(new MonitoredAccount
            {
                CTraderAccount = CTraderAccounts.First(e => e.Id == 3),
                Monitor = Monitors.First(e => e.Id == 2),
                ExpectedContracts = 3000
            });
            MonitoredAccounts.Add(new MonitoredAccount
            {
                CTraderAccount = CTraderAccounts.First(e => e.Id == 4),
                Monitor = Monitors.First(e => e.Id == 2),
                ExpectedContracts = 4000
            });

            SaveChanges();

            TradingAccounts.Add(new TradingAccount
            {
                Profile = Profiles.First(p => p.Description == "Dummy Profile 1"),
                Expert = Experts.First(),
                MetaTraderAccount = MetaTraderAccounts.First(e => e.Id == 2),
                ShouldTrade = true
            });
            TradingAccounts.Add(new TradingAccount
            {
                Profile = Profiles.First(p => p.Description == "Dummy Profile 2"),
                Expert = Experts.First(),
                MetaTraderAccount = MetaTraderAccounts.First(e => e.Id == 3),
            });

            SaveChanges();

            var i = 1;
            AddSimpleExpertSet("NZDUSD+", "AUDUSD+", i++);
            AddSimpleExpertSet("AUDNZD+", "NZDUSD+", i++);
            AddSimpleExpertSet("AUDUSD+", "AUDNZD+", i++);
            AddSimpleExpertSet("EURAUD+", "EURNZD+", i++);
            AddSimpleExpertSet("AUDNZD+", "EURNZD+", i++);
            AddSimpleExpertSet("AUDNZD+", "EURAUD+", i++);
            AddSimpleExpertSet("EURAUD+", "EURUSD+", i++);
            AddSimpleExpertSet("EURUSD+", "AUDUSD+", i++);
            AddSimpleExpertSet("AUDUSD+", "EURAUD+", i++);
            AddSimpleExpertSet("NZDUSD+", "EURUSD+", i++);
            AddSimpleExpertSet("EURNZD+", "NZDUSD+", i++);

            SaveChanges();
        }

        private void AddSimpleExpertSet(string symbol1, string symbol2, int magic)
        {
            ExpertSets.Add(new ExpertSet
            {
                Description = $"{symbol1} {symbol2}",
                TradingAccount = TradingAccounts.First(e => e.Id == 2),
                TimeFrame = ExpertSet.TimeFrames.M1,
                Symbol1 = symbol1,
                Symbol2 = symbol2,
                Variant = ExpertSet.Variants.NormalNormalBase,
                Delta = 10,
                M = 0.6,
                Diff = 20,
                Period = 3,
                Tp1 = 20,
                MagicNumber = magic * 100,
                LotSize = 0.01,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                ProfitCloseValueSell = 100,
                ProfitCloseValueBuy = 100,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                MaxTradeSetCount = 600,
                Last24HMaxOpen = 200,
                ReOpenDiff = 20,
                ReOpenDiffChangeCount = 3,
                ReOpenDiff2 = 30,
                ShouldRun = true
            });
        }

        private void InitQuadro()
        {
            var mt4Platform = MetaTraderPlatforms.First(p => p.Description == "AuraFX-Demo");
            var mt4Acccount = MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Quadro Account",
                User = 8801896,
                Password = "dzd5hts",
                MetaTraderPlatform = mt4Platform,
                ShouldConnect = true
            });

            var profile = Profiles.Add(new Profile { Description = "Quadro Profile" });
            var ta = TradingAccounts
                .Add(new TradingAccount
                {
                    ShouldTrade = true,
                    Profile = profile,
                    Expert = Experts.First(),
                    MetaTraderAccount = mt4Acccount
                });

            var i = 1;
            AddProdSet(ta, "01 AU NU", "NZDUSD+", "AUDUSD+", 1, 4, 50, 300, 1000, 30, 1500, 150, 2, 10 * i++);
            AddProdSet(ta, "02 AN NU", "AUDNZD+", "NZDUSD+", 3, 1, 75, 400, 1250, 100, 1250, 200, 1.3, 10 * i++);
            AddProdSet(ta, "03 AN AU", "AUDUSD+", "AUDNZD+", 1, 1, 200, 300, 1500, 100, 1500, 200, 1.6, 10 * i++);
            AddProdSet(ta, "04 EA EN", "EURNZD+", "EURAUD+", 1, 1, 75, 200, 1750, 100, 1750, 200, 1.6, 10 * i++);
            AddProdSet(ta, "05 AN EN", "AUDNZD+", "EURNZD+", 1, 1, 75, 400, 1250, 100, 1250, 200, 0.7, 10 * i++);
            AddProdSet(ta, "06 EA AN", "AUDNZD+", "EURAUD+", 3, 1, 75, 100, 1250, 100, 1250, 150, 1.2, 10 * i++);
            AddProdSet(ta, "07 EU EA", "EURAUD+", "EURUSD+", 1, 1, 50, 200, 1000, 100, 1000, 200, 0.6, 10 * i++);
            AddProdSet(ta, "08 AU EU", "EURUSD+", "AUDUSD+", 1, 1, 125, 800, 2000, 200, 1500, 200, 1.4, 10 * i++);
            AddProdSet(ta, "09 AU EA", "EURAUD+", "AUDUSD+", 1, 1, 125, 900, 2500, 100, 2500, 200, 1.4, 10 * i++);
            AddProdSet(ta, "10 EU NU", "NZDUSD+", "EURUSD+", 1, 1, 75, 500, 1750, 100, 1750, 200, 1.2, 10 * i++);
            AddProdSet(ta, "11 EN NU", "EURNZD+", "NZDUSD+", 3, 1, 75, 1000, 3000, 30, 1500, 200, 1.5, 10 * i++);
            AddProdSet(ta, "12 EN EU", "EURUSD+", "EURNZD+", 1, 1, 100, 200, 1250, 100, 1250, 200, 1.6, 10 * i++);
            AddProdSet(ta, "13 AN NC", "AUDNZD+", "NZDCAD+", 1, 1, 75, 800, 1500, 100, 1500, 200, 1.6, 10 * i++);
            AddProdSet(ta, "14 AC NC", "NZDCAD+", "AUDCAD+", 1, 1, 75, 700, 1750, 30, 1500, 200, 1.6, 10 * i++);

            AddProdSet(ta, "16 AU UC", "AUDUSD+", "USDCAD+", 3, 1, 50, 100, 1000, 100, 1000, 150, 0.6, 10 * i++);
            AddProdSet(ta, "17 AC UC", "AUDCAD+", "USDCAD+", 1, 1, 100, 400, 1000, 100, 1000, 200, 1.2, 10 * i++);
            AddProdSet(ta, "18 AC AU", "AUDCAD+", "AUDUSD+", 1, 1, 50, 100, 750, 100, 750, 150, 1.9, 10 * i++);
            AddProdSet(ta, "19 EA EC", "EURCAD+", "EURAUD+", 1, 1, 75, 600, 1250, 100, 1250, 200, 1.6, 10 * i++);


            AddProdSet(ta, "22 EN NC", "NZDCAD+", "EURNZD+", 3, 1, 75, 300, 750, 100, 750, 150, 1.2, 10 * i++);
            AddProdSet(ta, "23 EC NC", "EURCAD+", "NZDCAD+", 3, 1, 100, 300, 2750, 30, 1500, 200, 1.5, 10 * i++);
            AddProdSet(ta, "24 EC EN", "EURCAD+", "EURNZD+", 1, 1, 75, 1000, 2750, 30, 1500, 200, 0.9, 10 * i++);
            AddProdSet(ta, "25 EU UC", "USDCAD+", "EURUSD+", 3, 1, 75, 200, 1000, 30, 1000, 200, 1.2, 10 * i++);
            AddProdSet(ta, "26 EC UC", "EURCAD+", "USDCAD+", 1, 1, 75, 500, 2750, 30, 2750, 200, 1.4, 10 * i++);
            AddProdSet(ta, "27 EC EU", "EURCAD+", "EURUSD+", 1, 1, 50, 100, 1000, 30, 1000, 200, 1.9, 10 * i++);
            AddProdSet(ta, "29 UC NC", "NZDCAD+", "USDCAD+", 1, 1, 25, 300, 1750, 30, 1500, 150, 1.2, 10 * i++);
            AddProdSet(ta, "30 NC NU", "NZDCAD+", "NZDUSD+", 1, 1, 50, 300, 2250, 30, 2250, 200, 0.9, 10 * i++);
        }

        private void AddProdSet(TradingAccount ta, string desc, string sym1, string sym2, int varId, int diff, int per, int tp1,
            int reOpenDiff, int reChangeCount, int reOpenDiff2, int delta, double m, int magic)
        {
            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = ta,
                Description = desc,
                MagicNumber = magic,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = sym1,
                Symbol2 = sym2,
                Variant = (ExpertSet.Variants)varId,
                Diff = diff,
                Period = per,
                LotSize = 0.01,
                Tp1 = tp1,
                ReOpenDiff = reOpenDiff,
                ReOpenDiffChangeCount = reChangeCount,
                ReOpenDiff2 = reOpenDiff2,
                Delta = delta,
                M = m,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                MaxTradeSetCount = 1000,
                Last24HMaxOpen = 1000
            });
        }
    }
}
