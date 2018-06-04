namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Run = c.Boolean(nullable: false),
                        CTraderAccountId = c.Int(),
                        MetaTraderAccountId = c.Int(),
                        FixTraderAccountId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CTraderAccounts", t => t.CTraderAccountId)
                .ForeignKey("dbo.FixTraderAccounts", t => t.FixTraderAccountId)
                .ForeignKey("dbo.MetaTraderAccounts", t => t.MetaTraderAccountId)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.CTraderAccountId)
                .Index(t => t.MetaTraderAccountId)
                .Index(t => t.FixTraderAccountId);
            
            CreateTable(
                "dbo.CTraderAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.Long(nullable: false),
                        AccessToken = c.String(nullable: false),
                        CTraderPlatformId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CTraderPlatforms", t => t.CTraderPlatformId, cascadeDelete: true)
                .Index(t => t.CTraderPlatformId);
            
            CreateTable(
                "dbo.CTraderPlatforms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountsApi = c.String(nullable: false),
                        TradingHost = c.String(nullable: false),
                        ClientId = c.String(nullable: false),
                        Secret = c.String(nullable: false),
                        Playground = c.String(nullable: false),
                        AccessBaseUrl = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FixTraderAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(nullable: false),
                        CommandSocketPort = c.Int(nullable: false),
                        EventsSocketPort = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MetaTraderAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User = c.Long(nullable: false),
                        Password = c.String(nullable: false),
                        MetaTraderPlatformId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MetaTraderPlatforms", t => t.MetaTraderPlatformId, cascadeDelete: true)
                .Index(t => t.MetaTraderPlatformId);
            
            CreateTable(
                "dbo.MetaTraderPlatforms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SrvFilePath = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Masters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Run = c.Boolean(nullable: false),
                        AccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: false)
                .Index(t => t.ProfileId)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.Slaves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Run = c.Boolean(nullable: false),
                        MasterId = c.Int(nullable: false),
                        AccountId = c.Int(nullable: false),
                        SymbolSuffix = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.Masters", t => t.MasterId, cascadeDelete: false)
                .Index(t => t.MasterId)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.Copiers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SlaveId = c.Int(nullable: false),
                        Run = c.Boolean(nullable: false),
                        CopyRatio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderType = c.Int(nullable: false),
                        SlippageInPips = c.Int(nullable: false),
                        MaxRetryCount = c.Int(nullable: false),
                        RetryPeriodInMilliseconds = c.Int(nullable: false),
                        DelayInMilliseconds = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Slaves", t => t.SlaveId, cascadeDelete: true)
                .Index(t => t.SlaveId);
            
            CreateTable(
                "dbo.FixApiCopiers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SlaveId = c.Int(nullable: false),
                        Run = c.Boolean(nullable: false),
                        CopyRatio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderType = c.Int(nullable: false),
                        Slippage = c.Double(nullable: false),
                        BurstPeriodInMilliseconds = c.Int(nullable: false),
                        DelayInMilliseconds = c.Int(nullable: false),
                        MaxRetryCount = c.Int(nullable: false),
                        RetryPeriodInMilliseconds = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Slaves", t => t.SlaveId, cascadeDelete: true)
                .Index(t => t.SlaveId);
            
            CreateTable(
                "dbo.SymbolMappings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SlaveId = c.Int(nullable: false),
                        From = c.String(),
                        To = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Slaves", t => t.SlaveId, cascadeDelete: true)
                .Index(t => t.SlaveId);
            
            CreateTable(
                "dbo.Pushings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        FutureAccountId = c.Int(nullable: false),
                        FutureSymbol = c.String(nullable: false),
                        AlphaMasterId = c.Int(nullable: false),
                        AlphaSymbol = c.String(nullable: false),
                        BetaMasterId = c.Int(nullable: false),
                        BetaSymbol = c.String(nullable: false),
                        HedgeAccountId = c.Int(nullable: false),
                        HedgeSymbol = c.String(nullable: false),
                        PushingDetailId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AlphaMasterId, cascadeDelete: false)
                .ForeignKey("dbo.Accounts", t => t.BetaMasterId, cascadeDelete: false)
                .ForeignKey("dbo.Accounts", t => t.FutureAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.HedgeAccountId, cascadeDelete: false)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: false)
                .ForeignKey("dbo.PushingDetails", t => t.PushingDetailId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.FutureAccountId)
                .Index(t => t.AlphaMasterId)
                .Index(t => t.BetaMasterId)
                .Index(t => t.HedgeAccountId)
                .Index(t => t.PushingDetailId);
            
            CreateTable(
                "dbo.PushingDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SmallContractSize = c.Int(nullable: false),
                        BigContractSize = c.Int(nullable: false),
                        BigPercentage = c.Int(nullable: false),
                        FutureOpenDelayInMs = c.Int(nullable: false),
                        MinIntervalInMs = c.Int(nullable: false),
                        MaxIntervalInMs = c.Int(nullable: false),
                        HedgeSignalContractLimit = c.Int(nullable: false),
                        MasterSignalContractLimit = c.Int(nullable: false),
                        FullContractSize = c.Int(nullable: false),
                        AlphaLots = c.Double(nullable: false),
                        BetaLots = c.Double(nullable: false),
                        HedgeLots = c.Double(nullable: false),
                        MaxRetryCount = c.Int(nullable: false),
                        RetryPeriodInMilliseconds = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StratDealingArbs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        ContractSize = c.Int(nullable: false),
                        Lots = c.Double(nullable: false),
                        MaxNumberOfPositions = c.Int(nullable: false),
                        SignalDiffInPip = c.Double(nullable: false),
                        SignalStepInPip = c.Double(nullable: false),
                        TargetInPip = c.Double(nullable: false),
                        MinOpenTimeInMinutes = c.Int(nullable: false),
                        EarliestOpenTime = c.Time(nullable: false, precision: 7),
                        LatestOpenTime = c.Time(nullable: false, precision: 7),
                        LatestCloseTime = c.Time(nullable: false, precision: 7),
                        FtAccountId = c.Int(nullable: false),
                        FtSymbol = c.String(nullable: false),
                        MtAccountId = c.Int(nullable: false),
                        MtSymbol = c.String(nullable: false),
                        ShiftInPip = c.Double(),
                        ShiftCalcInterval = c.Time(nullable: false, precision: 7),
                        PipSize = c.Double(nullable: false),
                        MagicNumber = c.Int(nullable: false),
                        MaxRetryCount = c.Int(nullable: false),
                        RetryPeriodInMilliseconds = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.FtAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.MtAccountId, cascadeDelete: false)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: false)
                .Index(t => t.ProfileId)
                .Index(t => t.FtAccountId)
                .Index(t => t.MtAccountId);
            
            CreateTable(
                "dbo.Tickers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        MainAccountId = c.Int(nullable: false),
                        MainSymbol = c.String(),
                        PairAccountId = c.Int(),
                        PairSymbol = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.MainAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.PairAccountId)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: false)
                .Index(t => t.ProfileId)
                .Index(t => t.MainAccountId)
                .Index(t => t.PairAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickers", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Tickers", "PairAccountId", "dbo.Accounts");
            DropForeignKey("dbo.Tickers", "MainAccountId", "dbo.Accounts");
            DropForeignKey("dbo.StratDealingArbs", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.StratDealingArbs", "MtAccountId", "dbo.Accounts");
            DropForeignKey("dbo.StratDealingArbs", "FtAccountId", "dbo.Accounts");
            DropForeignKey("dbo.Pushings", "PushingDetailId", "dbo.PushingDetails");
            DropForeignKey("dbo.Pushings", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Pushings", "HedgeAccountId", "dbo.Accounts");
            DropForeignKey("dbo.Pushings", "FutureAccountId", "dbo.Accounts");
            DropForeignKey("dbo.Pushings", "BetaMasterId", "dbo.Accounts");
            DropForeignKey("dbo.Pushings", "AlphaMasterId", "dbo.Accounts");
            DropForeignKey("dbo.SymbolMappings", "SlaveId", "dbo.Slaves");
            DropForeignKey("dbo.Slaves", "MasterId", "dbo.Masters");
            DropForeignKey("dbo.FixApiCopiers", "SlaveId", "dbo.Slaves");
            DropForeignKey("dbo.Copiers", "SlaveId", "dbo.Slaves");
            DropForeignKey("dbo.Slaves", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Masters", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Masters", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Accounts", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Accounts", "MetaTraderAccountId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.MetaTraderAccounts", "MetaTraderPlatformId", "dbo.MetaTraderPlatforms");
            DropForeignKey("dbo.Accounts", "FixTraderAccountId", "dbo.FixTraderAccounts");
            DropForeignKey("dbo.Accounts", "CTraderAccountId", "dbo.CTraderAccounts");
            DropForeignKey("dbo.CTraderAccounts", "CTraderPlatformId", "dbo.CTraderPlatforms");
            DropIndex("dbo.Tickers", new[] { "PairAccountId" });
            DropIndex("dbo.Tickers", new[] { "MainAccountId" });
            DropIndex("dbo.Tickers", new[] { "ProfileId" });
            DropIndex("dbo.StratDealingArbs", new[] { "MtAccountId" });
            DropIndex("dbo.StratDealingArbs", new[] { "FtAccountId" });
            DropIndex("dbo.StratDealingArbs", new[] { "ProfileId" });
            DropIndex("dbo.Pushings", new[] { "PushingDetailId" });
            DropIndex("dbo.Pushings", new[] { "HedgeAccountId" });
            DropIndex("dbo.Pushings", new[] { "BetaMasterId" });
            DropIndex("dbo.Pushings", new[] { "AlphaMasterId" });
            DropIndex("dbo.Pushings", new[] { "FutureAccountId" });
            DropIndex("dbo.Pushings", new[] { "ProfileId" });
            DropIndex("dbo.SymbolMappings", new[] { "SlaveId" });
            DropIndex("dbo.FixApiCopiers", new[] { "SlaveId" });
            DropIndex("dbo.Copiers", new[] { "SlaveId" });
            DropIndex("dbo.Slaves", new[] { "AccountId" });
            DropIndex("dbo.Slaves", new[] { "MasterId" });
            DropIndex("dbo.Masters", new[] { "AccountId" });
            DropIndex("dbo.Masters", new[] { "ProfileId" });
            DropIndex("dbo.MetaTraderAccounts", new[] { "MetaTraderPlatformId" });
            DropIndex("dbo.CTraderAccounts", new[] { "CTraderPlatformId" });
            DropIndex("dbo.Accounts", new[] { "FixTraderAccountId" });
            DropIndex("dbo.Accounts", new[] { "MetaTraderAccountId" });
            DropIndex("dbo.Accounts", new[] { "CTraderAccountId" });
            DropIndex("dbo.Accounts", new[] { "ProfileId" });
            DropTable("dbo.Tickers");
            DropTable("dbo.StratDealingArbs");
            DropTable("dbo.PushingDetails");
            DropTable("dbo.Pushings");
            DropTable("dbo.SymbolMappings");
            DropTable("dbo.FixApiCopiers");
            DropTable("dbo.Copiers");
            DropTable("dbo.Slaves");
            DropTable("dbo.Masters");
            DropTable("dbo.Profiles");
            DropTable("dbo.MetaTraderPlatforms");
            DropTable("dbo.MetaTraderAccounts");
            DropTable("dbo.FixTraderAccounts");
            DropTable("dbo.CTraderPlatforms");
            DropTable("dbo.CTraderAccounts");
            DropTable("dbo.Accounts");
        }
    }
}
