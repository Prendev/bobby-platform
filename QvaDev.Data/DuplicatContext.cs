using System.Configuration;
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

                if (!Experts.Any(e => e.Description == "Quadro"))
                    Experts.Add(new Expert { Description = "Quadro" });
                //if (!Experts.Any(e => e.Description == "Pushing"))
                //    Experts.Add(new Expert { Description = "Pushing" });

                SaveChanges();
            }
            catch  { }

            if (exists) return;
            bool create;
            if (bool.TryParse(ConfigurationManager.AppSettings["ShouldCreateDemoDatabase"], out create) && !create) return;

            InitDebug();
            SaveChanges();
            InitPushing();
            SaveChanges();
            Init10KSet();
            SaveChanges();
            Init30KSet();
            SaveChanges();
            Init60KSet();
            SaveChanges();
        }

        private void InitDebug()
        {
            var cTraderPlatform = CTraderPlatforms.Add(new CTraderPlatform()
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

            var mt4Platform = MetaTraderPlatforms.First(p => p.Description == "AuraFX-Demo");

            var mt4Account1 = MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Aura1",
                User = 8801929,
                Password = "7btmtdh",
                MetaTraderPlatform = mt4Platform
            });
            var mt4Account2 = MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Aura2",
                User = 8801895,
                Password = "v1iaxwc",
                MetaTraderPlatform = mt4Platform
            });

            var ctAccount1 = CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader1",
                AccountNumber = 3174636,
                CTraderPlatform = cTraderPlatform,
                AccessToken = "sVP14Z83qW455RoXW_c2dDvoF4PiIn-5-XxFVd2kwjs"
            });
            var ctAccount2 = CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader2",
                AccountNumber = 3174645,
                CTraderPlatform = cTraderPlatform,
                AccessToken = "sVP14Z83qW455RoXW_c2dDvoF4PiIn-5-XxFVd2kwjs"
            });
            var ctAccount3 = CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader3",
                AccountNumber = 3175387,
                CTraderPlatform = cTraderPlatform,
                AccessToken = "sVP14Z83qW455RoXW_c2dDvoF4PiIn-5-XxFVd2kwjs"
            });
            var ctAccount4 = CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader4",
                AccountNumber = 3175388,
                CTraderPlatform = cTraderPlatform,
                AccessToken = "sVP14Z83qW455RoXW_c2dDvoF4PiIn-5-XxFVd2kwjs"
            });

            var profile1 = Profiles.Add(new Profile() { Description = "Dummy Profile 1" });
            var profile2 = Profiles.Add(new Profile() { Description = "Dummy Profile 2" });

            var master1 = Masters.Add(new Master { Profile = profile1, MetaTraderAccount = mt4Account1 });
            var master2 = Masters.Add(new Master { Profile = profile2, MetaTraderAccount = mt4Account2 });

            var slave1 = Slaves.Add(new Slave { Master = master1, CTraderAccount = ctAccount1 });
            var slave2 = Slaves.Add(new Slave { Master = master1, CTraderAccount = ctAccount2 });
            var slave3 = Slaves.Add(new Slave { Master = master2, CTraderAccount = ctAccount3 });
            var slave4 = Slaves.Add(new Slave { Master = master2, CTraderAccount = ctAccount4 });

            SymbolMappings.Add(new SymbolMapping { Slave = slave1, From = "GER30.pri", To = "GERMAN30" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave1, From = "UK100.pri", To = "UK100" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave1, From = "US30.pri", To = "US30.PRM" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave1, From = "XAUUSD.pri", To = "XAUUSD" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave1, From = "EURUSD+", To = "EURUSD" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave2, From = "GER30.ex", To = "GERMAN30" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave2, From = "UK100.ex", To = "UK100" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave2, From = "US30.ex", To = "US30.PRM" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave2, From = "XAUUSD.ex", To = "XAUUSD" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave2, From = "EURUSD+", To = "EURUSD" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave3, From = "GER30.ex", To = "GERMAN30" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave3, From = "UK100.ex", To = "UK100" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave3, From = "US30.ex", To = "US30.PRM" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave3, From = "XAUUSD.ex", To = "XAUUSD" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave3, From = "EURUSD+", To = "EURUSD" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave4, From = "GER30.ex", To = "GERMAN30" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave4, From = "UK100.ex", To = "UK100" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave4, From = "US30.ex", To = "US30.PRM" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave4, From = "XAUUSD.ex", To = "XAUUSD" });
            SymbolMappings.Add(new SymbolMapping { Slave = slave4, From = "EURUSD+", To = "EURUSD" });

            Copiers.Add(new Copier
            {
                Slave = slave1,
                CopyRatio = 1,
                SlippageInPips = 30,
                MaxRetryCount = 5,
                RetryPeriodInMilliseconds = 25
            });
            Copiers.Add(new Copier
            {
                Slave = slave2,
                CopyRatio = 1,
                SlippageInPips = 30,
                MaxRetryCount = 5,
                RetryPeriodInMilliseconds = 3000
            });
            Copiers.Add(new Copier
            {
                Slave = slave3,
                CopyRatio = 1,
                SlippageInPips = 30,
                MaxRetryCount = 5,
                RetryPeriodInMilliseconds = 3000
            });
            Copiers.Add(new Copier
            {
                Slave = slave4,
                CopyRatio = 1,
                SlippageInPips = 30,
                MaxRetryCount = 5,
                RetryPeriodInMilliseconds = 3000
            });

            var monitor1 = Monitors.Add(new Monitor { Description = "Side A", Profile = profile1, Symbol = "EURUSD" });
            var monitor2 = Monitors.Add(new Monitor { Description = "Side B", Profile = profile1, Symbol = "EURUSD" });
            Monitors.Add(new Monitor { Description = "Side C", Profile = profile1, Symbol = "EURUSD" });
            Monitors.Add(new Monitor { Description = "Side D", Profile = profile1, Symbol = "EURUSD" });

            MonitoredAccounts.Add(new MonitoredAccount
            {
                MetaTraderAccount = mt4Account1,
                Monitor = monitor1,
                ExpectedContracts = 1000,
                Symbol = "EURUSD+",
                IsMaster = true
            });
            MonitoredAccounts.Add(new MonitoredAccount
            {
                CTraderAccount = ctAccount1,
                Monitor = monitor1,
                ExpectedContracts = 1000
            });
            MonitoredAccounts.Add(new MonitoredAccount
            {
                CTraderAccount = ctAccount2,
                Monitor = monitor1,
                ExpectedContracts = 2000
            });

            MonitoredAccounts.Add(new MonitoredAccount
            {
                MetaTraderAccount = mt4Account2,
                Monitor = monitor2,
                ExpectedContracts = 1000,
                Symbol = "EURUSD+",
                IsMaster = true
            });
            MonitoredAccounts.Add(new MonitoredAccount
            {
                CTraderAccount = ctAccount3,
                Monitor = monitor2,
                ExpectedContracts = 3000
            });
            MonitoredAccounts.Add(new MonitoredAccount
            {
                CTraderAccount = ctAccount4,
                Monitor = monitor2,
                ExpectedContracts = 4000
            });

            var tAccount = TradingAccounts.Add(new TradingAccount
            {
                Description = "TTA1",
                Profile = profile1,
                Expert = Experts.First(),
                MetaTraderAccount = mt4Account1,
                ShouldTrade = true,
                TradeSetFloatingSwitch = -100000
            });
            TradingAccounts.Add(new TradingAccount
            {
                Description = "TTA2",
                Profile = profile2,
                Expert = Experts.First(),
                MetaTraderAccount = mt4Account2,
                TradeSetFloatingSwitch = -100000
            });

            var i = 1;
            AddSimpleExpertSet(tAccount, "NZDUSD+", "AUDUSD+", i++);
            AddSimpleExpertSet(tAccount, "AUDNZD+", "NZDUSD+", i++);
            AddSimpleExpertSet(tAccount, "AUDUSD+", "AUDNZD+", i++);
            AddSimpleExpertSet(tAccount, "EURAUD+", "EURNZD+", i++);
            AddSimpleExpertSet(tAccount, "AUDNZD+", "EURNZD+", i++);
            AddSimpleExpertSet(tAccount, "AUDNZD+", "EURAUD+", i++);
            AddSimpleExpertSet(tAccount, "EURAUD+", "EURUSD+", i++);
            AddSimpleExpertSet(tAccount, "EURUSD+", "AUDUSD+", i++);
            AddSimpleExpertSet(tAccount, "AUDUSD+", "EURAUD+", i++);
            AddSimpleExpertSet(tAccount, "NZDUSD+", "EURUSD+", i++);
            AddSimpleExpertSet(tAccount, "EURNZD+", "NZDUSD+", i++);
        }

        private void AddSimpleExpertSet(TradingAccount ta, string symbol1, string symbol2, int magic)
        {
            QuadroSets.Add(new QuadroSet
            {
                Description = $"{symbol1} {symbol2}",
                TradingAccount = ta,
                TimeFrame = QuadroSet.TimeFrames.M1,
                Symbol1 = symbol1,
                Symbol2 = symbol2,
                Variant = QuadroSet.Variants.NormalNormalBase,
                Delta = 10,
                M = 0.6,
                Diff = 20,
                Period = 3,
                Tp1 = 20,
                MagicNumber = magic * 100,
                LotSize = 0.05,
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

        private void InitPushing()
        {
            var profile1 = Profiles.First(p => p.Description == "Dummy Profile 1");
            var mt4Platform = MetaTraderPlatforms.First(p => p.Description == "BlackwellGlobal1-Demo5");

            var alphaMaster = MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Alpha Master",
                User = 518841,
                Password = "ri3eyef",
                MetaTraderPlatform = mt4Platform,
                Run = true
            });
            var betaMaster = MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Beta Master",
                User = 518842,
                Password = "4hjyebx",
                MetaTraderPlatform = mt4Platform,
                Run = true
            });
            var hedgeAccount = MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Hedge Account",
                User = 518843,
                Password = "ofkw7co",
                MetaTraderPlatform = mt4Platform,
                Run = true
            });

            var ftAccount = FixTraderAccounts.Add(new FixTraderAccount
            {
                Description = "FIX Trader priorfx",
                IpAddress = "192.81.110.211",
                CommandSocketPort = 9001,
                EventsSocketPort = 9002,
                Run = true
            });

            Pushings.Add(new Pushing
            {
                Profile = profile1,
                FutureAccount = ftAccount,
                FutureSymbol = "EURUSD",
                Description = "Pushing stuff",
                AlphaMaster = alphaMaster,
                AlphaSymbol = "EURUSD",
                BetaMaster = betaMaster,
                BetaSymbol = "EURUSD",
                HedgeAccount = hedgeAccount,
                HedgeSymbol = "EURUSD",
                PushingDetail = new PushingDetail
                {
                    SmallContractSize = 1,
                    BigContractSize = 2,
                    BigPercentage = 70,
                    FutureOpenDelayInMs = 50,
                    MinIntervalInMs = 50,
                    MaxIntervalInMs = 100,
                    AlphaLots = 1,
	                BetaLots = 1,
					HedgeLots = 2,
                    HedgeSignalContractLimit = -20,
                    MasterSignalContractLimit = -30,
                    FullContractSize = 200
                }
            });
        }

        private void Init10KSet()
        {
            var mt4Platform = MetaTraderPlatforms.First(p => p.Description == "AuraFX-Demo");
            var mt4Acccount = MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Quadro 10K",
                User = 8801927,
                Password = "eey3pca",
                MetaTraderPlatform = mt4Platform,
            });

            var profile = Profiles.Add(new Profile { Description = "Quadro 10K" });
            var ta = TradingAccounts
                .Add(new TradingAccount
                {
                    ShouldTrade = true,
                    Description = "10K",
                    Profile = profile,
                    Expert = Experts.First(),
                    MetaTraderAccount = mt4Acccount,
                    TradeSetFloatingSwitch = -2000
                });

            var i = 1;
            AddProdSet(ta, "QVA_01_AU_NU", "NZDUSD+", "AUDUSD+", 1, 4, 50, 0.02, 300, 1000, 4, 2000, 150, 2, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_02_AN_NU", "AUDNZD+", "NZDUSD+", 3, 1, 75, 0.04, 400, 1250, 3, 2000, 200, 1.3, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_03_AN_AU", "AUDUSD+", "AUDNZD+", 1, 1, 200, 0.06, 200, 1500, 3, 2200, 200, 1.6, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_04_EA_EN", "EURNZD+", "EURAUD+", 1, 1, 75, 0.04, 200, 1750, 3, 2250, 200, 1.6, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_05_AN_EN", "AUDNZD+", "EURNZD+", 1, 1, 75, 0.02, 400, 1250, 5, 1750, 200, 0.7, 5 * i++, 2, 8);
            AddProdSet(ta, "QVA_06_EA_AN", "AUDNZD+", "EURAUD+", 3, 1, 75, 0.02, 100, 1250, 4, 1750, 150, 1.2, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_07_EU_EA", "EURAUD+", "EURUSD+", 1, 1, 50, 0.04, 200, 1000, 4, 2000, 200, 0.6, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_08_AU_EU", "EURUSD+", "AUDUSD+", 1, 1, 125, 0.02, 800, 2000, 5, 3000, 200, 1.4, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_09_AU_EA", "EURAUD+", "AUDUSD+", 3, 1, 125, 0.02, 900, 2500, 4, 3000, 200, 1.4, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_10_EU_NU", "NZDUSD+", "EURUSD+", 1, 1, 75, 0.04, 500, 1750, 3, 2250, 200, 1.2, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_11_EN_NU", "EURNZD+", "NZDUSD+", 3, 1, 75, 0.02, 1000, 3000, 5, 4000, 200, 1.2, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_12_EN_EU", "EURUSD+", "EURNZD+", 1, 1, 75, 0.06, 200, 3000, 2, 4000, 200, 0.7, 5 * i++, 2, 4);
        }

        private void Init30KSet()
        {
            var mt4Platform = MetaTraderPlatforms.First(p => p.Description == "AuraFX-Demo");
            var mt4Acccount = MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Quadro 30K",
                User = 8801928,
                Password = "4ouvpcu",
                MetaTraderPlatform = mt4Platform,
            });

            var profile = Profiles.Add(new Profile { Description = "Quadro 30K" });
            var ta = TradingAccounts
                .Add(new TradingAccount
                {
                    ShouldTrade = true,
                    Description = "30K",
                    Profile = profile,
                    Expert = Experts.First(),
                    MetaTraderAccount = mt4Acccount,
                    TradeSetFloatingSwitch = -5000
                });

            var i = 1;
            AddProdSet(ta, "QVA_01_AU_NU", "NZDUSD+", "AUDUSD+", 1, 4, 50, 0.02, 300, 1000, 4, 2000, 150, 2, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_02_AN_NU", "AUDNZD+", "NZDUSD+", 3, 1, 75, 0.04, 400, 1250, 3, 2000, 200, 1.3, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_03_AN_AU", "AUDUSD+", "AUDNZD+", 1, 1, 200, 0.04, 200, 1500, 3, 2200, 200, 1.6, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_04_EA_EN", "EURNZD+", "EURAUD+", 1, 1, 75, 0.04, 200, 1750, 3, 2250, 200, 1.6, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_05_AN_EN", "AUDNZD+", "EURNZD+", 1, 1, 75, 0.02, 400, 1250, 5, 1750, 200, 0.7, 5 * i++, 2, 8);
            AddProdSet(ta, "QVA_06_EA_AN", "AUDNZD+", "EURAUD+", 3, 1, 75, 0.04, 100, 1250, 4, 1750, 150, 1.2, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_07_EU_EA", "EURAUD+", "EURUSD+", 1, 1, 50, 0.04, 200, 1000, 4, 2000, 200, 0.6, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_08_AU_EU", "EURUSD+", "AUDUSD+", 1, 1, 125, 0.02, 800, 2000, 5, 3000, 200, 1.4, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_09_AU_EA", "EURAUD+", "AUDUSD+", 3, 1, 125, 0.02, 900, 2500, 4, 3000, 200, 1.4, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_10_EU_NU", "NZDUSD+", "EURUSD+", 1, 1, 75, 0.04, 500, 1750, 3, 2250, 200, 1.2, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_11_EN_NU", "EURNZD+", "NZDUSD+", 3, 1, 75, 0.02, 1000, 3000, 5, 4000, 200, 1.2, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_12_EN_EU", "EURUSD+", "EURNZD+", 1, 1, 75, 0.04, 200, 3000, 2, 4000, 200, 0.7, 5 * i++, 2, 4);
            AddProdSet(ta, "QVA_13_AN_NC", "AUDNZD+", "NZDCAD+", 3, 1, 75, 0.02, 800, 1500, 5, 2500, 200, 1.6, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_14_AC_NC", "NZDCAD+", "AUDCAD+", 1, 1, 75, 0.04, 700, 1750, 4, 2750, 200, 1.6, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_15_AC_AN", "AUDCAD+", "AUDNZD+", 1, 1, 25, 0.04, 100, 1000, 4, 2000, 150, 1.1, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_16_AU_UC", "USDCAD+", "AUDUSD+", 3, 1, 50, 0.04, 100, 1000, 4, 2000, 150, 0.6, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_17_AC_UC", "AUDCAD+", "USDCAD+", 1, 1, 100, 0.02, 400, 1000, 5, 2000, 150, 1.2, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_18_AC_AU", "AUDCAD+", "AUDUSD+", 1, 1, 50, 0.02, 100, 750, 5, 1750, 150, 1.9, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_19_EA_EC", "EURCAD+", "EURAUD+", 1, 1, 75, 0.02, 600, 1250, 5, 2250, 200, 1.6, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_20_EC_AC", "EURCAD+", "AUDCAD+", 1, 1, 75, 0.02, 600, 2500, 4, 3500, 200, 1.5, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_21_AC_EA", "AUDCAD+", "EURAUD+", 3, 1, 75, 0.04, 400, 1000, 4, 2000, 200, 1.4, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_22_EN_NC", "NZDCAD+", "EURNZD+", 3, 1, 75, 0.02, 300, 750, 5, 1750, 150, 1.2, 5 * i++, 2, 8);
            AddProdSet(ta, "QVA_23_EC_NC", "NZDCAD+", "EURCAD+", 1, 1, 100, 0.04, 300, 2750, 3, 3750, 200, 1.5, 5 * i++, 2, 5); // szarul fut élesben!!!
            AddProdSet(ta, "QVA_24_EC_EN", "EURCAD+", "EURNZD+", 1, 1, 75, 0.02, 1000, 2750, 3, 3750, 200, 0.9, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_25_EU_UC", "USDCAD+", "EURUSD+", 3, 1, 75, 0.02, 200, 1000, 4, 2000, 200, 1.2, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_26_EC_UC", "EURCAD+", "USDCAD+", 1, 1, 75, 0.02, 500, 2750, 4, 3750, 200, 1.4, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_27_EC_EU", "EURCAD+", "EURUSD+", 1, 1, 50, 0.02, 100, 1000, 5, 2000, 200, 1.9, 5 * i++, 2, 8);
            AddProdSet(ta, "QVA_28_NU_UC", "USDCAD+", "NZDUSD+", 3, 1, 75, 0.02, 400, 2000, 3, 3000, 200, 0.8, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_29_UC_NC", "NZDCAD+", "USDCAD+", 1, 1, 25, 0.02, 300, 1750, 4, 2750, 150, 1.2, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_30_NC_NU", "NZDCAD+", "NZDUSD+", 1, 1, 50, 0.04, 300, 2250, 2, 3250, 200, 0.9, 5 * i++, 2, 4);
        }

        private void Init60KSet()
        {
            var mt4Platform = MetaTraderPlatforms.First(p => p.Description == "AuraFX-Demo");
            var mt4Acccount = MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Quadro 60K",
                User = 8801912,
                Password = "ou1eshv",
                MetaTraderPlatform = mt4Platform
            });

            var profile = Profiles.Add(new Profile { Description = "Quadro 60K" });
            var ta = TradingAccounts
                .Add(new TradingAccount
                {
                    ShouldTrade = true,
                    Description = "30K",
                    Profile = profile,
                    Expert = Experts.First(),
                    MetaTraderAccount = mt4Acccount,
                    TradeSetFloatingSwitch = -10000
                });

            var i = 1;
            AddProdSet(ta, "QVA_01_AU_NU", "NZDUSD+", "AUDUSD+", 1, 4, 50, 0.1, 300, 1000, 4, 2000, 150, 2, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_02_AN_NU", "AUDNZD+", "NZDUSD+", 3, 1, 75, 0.16, 400, 1250, 3, 2000, 200, 1.3, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_03_AN_AU", "AUDUSD+", "AUDNZD+", 1, 1, 200, 0.16, 200, 1500, 3, 2200, 200, 1.6, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_04_EA_EN", "EURNZD+", "EURAUD+", 1, 1, 75, 0.16, 200, 1750, 3, 2250, 200, 1.6, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_05_AN_EN", "AUDNZD+", "EURNZD+", 1, 1, 75, 0.06, 400, 1250, 5, 1750, 200, 0.7, 5 * i++, 2, 8);
            AddProdSet(ta, "QVA_06_EA_AN", "AUDNZD+", "EURAUD+", 3, 1, 75, 0.12, 100, 1250, 4, 1750, 150, 1.2, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_07_EU_EA", "EURAUD+", "EURUSD+", 1, 1, 50, 0.14, 200, 1000, 4, 2000, 200, 0.6, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_08_AU_EU", "EURUSD+", "AUDUSD+", 1, 1, 125, 0.06, 800, 2000, 5, 3000, 200, 1.4, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_09_AU_EA", "EURAUD+", "AUDUSD+", 1, 1, 125, 0.06, 900, 2500, 4, 3000, 200, 1.4, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_10_EU_NU", "NZDUSD+", "EURUSD+", 1, 1, 75, 0.1, 500, 1750, 3, 2250, 200, 1.2, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_11_EN_NU", "EURNZD+", "NZDUSD+", 3, 1, 75, 0.06, 1000, 3000, 5, 4000, 200, 1.2, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_12_EN_EU", "EURUSD+", "EURNZD+", 1, 1, 75, 0.12, 200, 3000, 2, 4000, 200, 0.7, 5 * i++, 2, 4);
            AddProdSet(ta, "QVA_13_AN_NC", "AUDNZD+", "EURNZD+", 1, 1, 75, 0.06, 800, 1500, 5, 2500, 200, 1.6, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_14_AC_NC", "NZDCAD+", "AUDCAD+", 1, 1, 75, 0.12, 700, 1750, 4, 2750, 200, 1.6, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_15_AC_AN", "AUDCAD+", "AUDNZD+", 1, 1, 25, 0.16, 100, 1000, 4, 2000, 150, 1.1, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_16_AU_UC", "USDCAD+", "AUDUSD+", 1, 1, 50, 0.16, 100, 1000, 4, 2000, 150, 0.6, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_17_AC_UC", "AUDCAD+", "USDCAD+", 1, 1, 100, 0.04, 400, 1000, 5, 2000, 150, 1.2, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_18_AC_AU", "AUDCAD+", "AUDUSD+", 1, 1, 50, 0.08, 100, 750, 5, 1750, 150, 1.9, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_19_EA_EC", "EURCAD+", "EURAUD+", 1, 1, 75, 0.06, 600, 1250, 5, 2250, 200, 1.6, 5 * i++, 2, 7);
            AddProdSet(ta, "QVA_20_EC_AC", "EURCAD+", "AUDCAD+", 1, 1, 75, 0.06, 600, 2500, 4, 3500, 200, 1.5, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_21_AC_EA", "AUDCAD+", "EURAUD+", 1, 1, 75, 0.1, 400, 1000, 4, 2000, 200, 1.4, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_22_EN_NC", "NZDCAD+", "EURNZD+", 1, 1, 75, 0.06, 300, 750, 5, 1750, 150, 1.2, 5 * i++, 2, 8);
            AddProdSet(ta, "QVA_23_EC_NC", "NZDCAD+", "EURCAD+", 1, 1, 100, 0.12, 300, 2750, 3, 3750, 200, 1.5, 5 * i++, 2, 5); // szarul fut élesben!!!
            AddProdSet(ta, "QVA_24_EC_EN", "EURCAD+", "EURNZD+", 1, 1, 75, 0.1, 1000, 2750, 3, 3750, 200, 0.9, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_25_EU_UC", "USDCAD+", "EURUSD+", 1, 1, 75, 0.06, 200, 1000, 4, 2000, 200, 1.2, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_26_EC_UC", "EURCAD+", "USDCAD+", 1, 1, 75, 0.04, 500, 2750, 4, 3750, 200, 1.4, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_27_EC_EU", "EURCAD+", "EURUSD+", 1, 1, 50, 0.04, 100, 1000, 5, 2000, 200, 1.9, 5 * i++, 2, 8);
            AddProdSet(ta, "QVA_28_NU_UC", "USDCAD+", "NZDUSD+", 1, 1, 75, 0.04, 400, 2000, 3, 3000, 200, 0.8, 5 * i++, 2, 5);
            AddProdSet(ta, "QVA_29_UC_NC", "NZDCAD+", "USDCAD+", 1, 1, 25, 0.06, 300, 1750, 4, 2750, 150, 1.2, 5 * i++, 2, 6);
            AddProdSet(ta, "QVA_30_NC_NU", "NZDCAD+", "NZDUSD+", 1, 1, 50, 0.12, 300, 2250, 2, 3250, 200, 0.9, 5 * i++, 2, 4);
        }

        private void AddProdSet(TradingAccount ta, string desc, string sym1, string sym2, int varId, int diff, int per, double lots, int tp1,
            int reOpenDiff, int reChangeCount, int reOpenDiff2, int delta, double m, int magic, int last24 = 1000, int maxTrade = 1000)
        {
            QuadroSets.Add(new QuadroSet
            {
                ShouldRun = true,
                TradingAccount = ta,
                Description = desc,
                MagicNumber = magic,
                TimeFrame = QuadroSet.TimeFrames.M15,
                Symbol1 = sym1,
                Symbol2 = sym2,
                Variant = (QuadroSet.Variants)varId,
                Diff = diff,
                Period = per,
                LotSize = lots,
                Tp1 = tp1,
                ReOpenDiff = reOpenDiff,
                ReOpenDiffChangeCount = reChangeCount,
                ReOpenDiff2 = reOpenDiff2,
                Delta = delta,
                M = m,
                TradeOpeningEnabled = true,
                MaxTradeSetCount = maxTrade,
                Last24HMaxOpen = last24
            });
        }
    }
}
