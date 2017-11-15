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
                User = 8801562,
                Password = "k0agjjf",
                MetaTraderPlatform = mt4Platform,
                ShouldConnect = true
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
                MagicNumber = magic * 1000,
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
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge,
                ShouldRun = true
            });
        }

        private void InitQuadro()
        {
            var mt4Platform = MetaTraderPlatforms.First(p => p.Description == "AuraFX-Demo");
            var mt4Acccount = MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Quadro Account",
                User = 8801883,
                Password = "8bowmhv",
                MetaTraderPlatform = mt4Platform,
                ShouldConnect = true
            });

            var profile = Profiles.Add(new Profile { Description = "Quadro Profile" });
            var tradingAccount = TradingAccounts
                .Add(new TradingAccount
                {
                    ShouldTrade = true,
                    Profile = profile,
                    Expert = Experts.First(),
                    MetaTraderAccount = mt4Acccount
                });

            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = tradingAccount,
                Description = "1 AU NU 3.96",
                MagicNumber = 10,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = "NZDUSD+",
                Symbol2 = "AUDUSD+",
                Variant = ExpertSet.Variants.NormalNormalBase,
                Diff = 4,
                Period = 50,
                LotSize = 0.01,
                Tp1 = 300,
                ReOpenDiff = 1000,
                ReOpenDiffChangeCount = 30,
                ReOpenDiff2 = 1500,
                Delta = 150,
                M = 2,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                Last24HMaxOpen = 200,
                MaxTradeSetCount = 600,
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge
            });
            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = tradingAccount,
                Description = "2 AN NU 4.40",
                MagicNumber = 20,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = "AUDNZD+",
                Symbol2 = "NZDUSD+",
                Variant = ExpertSet.Variants.NormalInverseBase,
                Diff = 1,
                Period = 75,
                LotSize = 0.01,
                Tp1 = 400,
                ReOpenDiff = 1250,
                ReOpenDiffChangeCount = 100,
                ReOpenDiff2 = 1250,
                Delta = 200,
                M = 1.3,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                Last24HMaxOpen = 200,
                MaxTradeSetCount = 600,
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge
            });
            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = tradingAccount,
                Description = "3 AN AU 3.93",
                MagicNumber = 30,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = "AUDUSD+",
                Symbol2 = "AUDNZD+",
                Variant = ExpertSet.Variants.NormalNormalBase,
                Diff = 1,
                Period = 200,
                LotSize = 0.01,
                Tp1 = 200,
                ReOpenDiff = 1500,
                ReOpenDiffChangeCount = 100,
                ReOpenDiff2 = 1500,
                Delta = 200,
                M = 1.6,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                Last24HMaxOpen = 200,
                MaxTradeSetCount = 600,
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge
            });
            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = tradingAccount,
                Description = "4 EA EN 3.48",
                MagicNumber = 40,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = "EURAUD+",
                Symbol2 = "EURNZD+",
                Variant = ExpertSet.Variants.NormalNormalBase,
                Diff = 1,
                Period = 75,
                LotSize = 0.01,
                Tp1 = 200,
                ReOpenDiff = 1750,
                ReOpenDiffChangeCount = 100,
                ReOpenDiff2 = 1750,
                Delta = 200,
                M = 1.6,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                Last24HMaxOpen = 200,
                MaxTradeSetCount = 600,
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge
            });
            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = tradingAccount,
                Description = "5 AN EN 3.17",
                MagicNumber = 50,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = "AUDNZD+",
                Symbol2 = "EURNZD+",
                Variant = ExpertSet.Variants.NormalNormalBase,
                Diff = 1,
                Period = 75,
                LotSize = 0.01,
                Tp1 = 400,
                ReOpenDiff = 1250,
                ReOpenDiffChangeCount = 100,
                ReOpenDiff2 = 1250,
                Delta = 200,
                M = 0.7,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                Last24HMaxOpen = 200,
                MaxTradeSetCount = 600,
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge
            });
            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = tradingAccount,
                Description = "6 EA AN 3.61",
                MagicNumber = 60,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = "AUDNZD+",
                Symbol2 = "EURAUD+",
                Variant = ExpertSet.Variants.NormalInverseBase,
                Diff = 1,
                Period = 75,
                LotSize = 0.01,
                Tp1 = 100,
                ReOpenDiff = 1250,
                ReOpenDiffChangeCount = 100,
                ReOpenDiff2 = 1250,
                Delta = 200,
                M = 1.2,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                Last24HMaxOpen = 200,
                MaxTradeSetCount = 600,
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge
            });
            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = tradingAccount,
                Description = "7 EU EA 5.55",
                MagicNumber = 70,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = "EURAUD+",
                Symbol2 = "EURUSD+",
                Variant = ExpertSet.Variants.NormalNormalBase,
                Diff = 1,
                Period = 50,
                LotSize = 0.01,
                Tp1 = 200,
                ReOpenDiff = 1000,
                ReOpenDiffChangeCount = 100,
                ReOpenDiff2 = 1000,
                Delta = 200,
                M = 0.6,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                Last24HMaxOpen = 200,
                MaxTradeSetCount = 600,
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge
            });
            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = tradingAccount,
                Description = "8 AU EU 3.36",
                MagicNumber = 80,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = "EURUSD+",
                Symbol2 = "AUDUSD+",
                Variant = ExpertSet.Variants.NormalNormalBase,
                Diff = 1,
                Period = 125,
                LotSize = 0.01,
                Tp1 = 800,
                ReOpenDiff = 2000,
                ReOpenDiffChangeCount = 200,
                ReOpenDiff2 = 1500,
                Delta = 200,
                M = 1.4,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                Last24HMaxOpen = 200,
                MaxTradeSetCount = 600,
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge
            });
            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = tradingAccount,
                Description = "9 AU EA 3.33",
                MagicNumber = 90,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = "AUDUSD+",
                Symbol2 = "EURAUD+",
                Variant = ExpertSet.Variants.NormalNormalBase,
                Diff = 1,
                Period = 125,
                LotSize = 0.01,
                Tp1 = 900,
                ReOpenDiff = 2500,
                ReOpenDiffChangeCount = 200,
                ReOpenDiff2 = 2500,
                Delta = 200,
                M = 1.4,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                Last24HMaxOpen = 200,
                MaxTradeSetCount = 600,
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge
            });
            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = tradingAccount,
                Description = "10 EU NU 4.13",
                MagicNumber = 100,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = "NZDUSD+",
                Symbol2 = "EURUSD+",
                Variant = ExpertSet.Variants.NormalNormalBase,
                Diff = 1,
                Period = 75,
                LotSize = 0.01,
                Tp1 = 500,
                ReOpenDiff = 1750,
                ReOpenDiffChangeCount = 200,
                ReOpenDiff2 = 1750,
                Delta = 200,
                M = 1.2,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                Last24HMaxOpen = 200,
                MaxTradeSetCount = 600,
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge
            });
            ExpertSets.Add(new ExpertSet
            {
                ShouldRun = true,
                TradingAccount = tradingAccount,
                Description = "11 EN NU 3.90",
                MagicNumber = 110,
                TimeFrame = ExpertSet.TimeFrames.M15,
                Symbol1 = "EURNZD+",
                Symbol2 = "NZDUSD+",
                Variant = ExpertSet.Variants.NormalInverseBase,
                Diff = 1,
                Period = 75,
                LotSize = 0.01,
                Tp1 = 1000,
                ReOpenDiff = 3000,
                ReOpenDiffChangeCount = 30,
                ReOpenDiff2 = 1500,
                Delta = 200,
                M = 1.5,
                TradeSetFloatingSwitch = -50000,
                TradeOpeningEnabled = true,
                HedgeProfitStop = 500,
                HedgeLossStop = 500,
                Last24HMaxOpen = 200,
                MaxTradeSetCount = 600,
                HedgeMode = ExpertSet.HedgeModes.TwoPairHedge
            });
        }
    }
}
