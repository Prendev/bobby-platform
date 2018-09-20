namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReInitForSqlite : DbMigration
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
                        FixApiAccountId = c.Int(),
                        IlyaFastFeedAccountId = c.Int(),
                        CqgClientApiAccountId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .ForeignKey("dbo.CqgClientApiAccounts", t => t.CqgClientApiAccountId)
                .ForeignKey("dbo.CTraderAccounts", t => t.CTraderAccountId)
                .ForeignKey("dbo.FixApiAccounts", t => t.FixApiAccountId)
                .ForeignKey("dbo.FixTraderAccounts", t => t.FixTraderAccountId)
                .ForeignKey("dbo.IlyaFastFeedAccounts", t => t.IlyaFastFeedAccountId)
                .ForeignKey("dbo.MetaTraderAccounts", t => t.MetaTraderAccountId)
                .Index(t => t.ProfileId)
                .Index(t => t.CTraderAccountId)
                .Index(t => t.MetaTraderAccountId)
                .Index(t => t.FixTraderAccountId)
                .Index(t => t.FixApiAccountId)
                .Index(t => t.IlyaFastFeedAccountId)
                .Index(t => t.CqgClientApiAccountId);
            
            CreateTable(
                "dbo.AggregatorAccounts",
                c => new
                    {
                        Aggregator_Id = c.Int(nullable: false),
                        Account_Id = c.Int(nullable: false),
                        Symbol = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => new { t.Aggregator_Id, t.Account_Id })
                .ForeignKey("dbo.Accounts", t => t.Account_Id, cascadeDelete: true)
                .ForeignKey("dbo.Aggregators", t => t.Aggregator_Id, cascadeDelete: true)
                .Index(t => t.Aggregator_Id)
                .Index(t => t.Account_Id);
            
            CreateTable(
                "dbo.Aggregators",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 2147483647),
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
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
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
                        SymbolSuffix = c.String(maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.Masters", t => t.MasterId, cascadeDelete: true)
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
                        RetryPeriodInMs = c.Int(nullable: false),
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
                        DelayInMilliseconds = c.Int(nullable: false),
                        MaxRetryCount = c.Int(nullable: false),
                        RetryPeriodInMs = c.Int(nullable: false),
                        SlippageInPip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeWindowInMs = c.Int(nullable: false),
                        PipSize = c.Decimal(nullable: false, precision: 18, scale: 5),
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
                        From = c.String(maxLength: 2147483647),
                        To = c.String(maxLength: 2147483647),
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
                        FutureExecutionMode = c.Int(nullable: false),
                        FutureAccountId = c.Int(nullable: false),
                        FutureSymbol = c.String(nullable: false, maxLength: 2147483647),
                        AlphaMasterId = c.Int(nullable: false),
                        AlphaSymbol = c.String(nullable: false, maxLength: 2147483647),
                        BetaMasterId = c.Int(nullable: false),
                        BetaSymbol = c.String(nullable: false, maxLength: 2147483647),
                        HedgeAccountId = c.Int(nullable: false),
                        HedgeSymbol = c.String(nullable: false, maxLength: 2147483647),
                        PushingDetailId = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AlphaMasterId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.BetaMasterId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.FutureAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.HedgeAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
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
                        EndingMinIntervalInMs = c.Int(nullable: false),
                        EndingMaxIntervalInMs = c.Int(nullable: false),
                        FullContractSize = c.Int(nullable: false),
                        PartialClosePercentage = c.Int(nullable: false),
                        AlphaLots = c.Double(nullable: false),
                        BetaLots = c.Double(nullable: false),
                        HedgeLots = c.Double(nullable: false),
                        MaxRetryCount = c.Int(nullable: false),
                        RetryPeriodInMs = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CqgClientApiAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 2147483647),
                        Password = c.String(nullable: false, maxLength: 2147483647),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CTraderAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.Long(nullable: false),
                        AccessToken = c.String(nullable: false, maxLength: 2147483647),
                        CTraderPlatformId = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CTraderPlatforms", t => t.CTraderPlatformId, cascadeDelete: true)
                .Index(t => t.CTraderPlatformId);
            
            CreateTable(
                "dbo.CTraderPlatforms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountsApi = c.String(nullable: false, maxLength: 2147483647),
                        TradingHost = c.String(nullable: false, maxLength: 2147483647),
                        ClientId = c.String(nullable: false, maxLength: 2147483647),
                        Secret = c.String(nullable: false, maxLength: 2147483647),
                        Playground = c.String(nullable: false, maxLength: 2147483647),
                        AccessBaseUrl = c.String(nullable: false, maxLength: 2147483647),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FixApiAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConfigPath = c.String(maxLength: 2147483647),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FixTraderAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(nullable: false, maxLength: 2147483647),
                        CommandSocketPort = c.Int(nullable: false),
                        EventsSocketPort = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IlyaFastFeedAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(nullable: false, maxLength: 2147483647),
                        Port = c.Int(nullable: false),
                        UserName = c.String(maxLength: 2147483647),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MetaTraderAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User = c.Long(nullable: false),
                        Password = c.String(nullable: false, maxLength: 2147483647),
                        MetaTraderPlatformId = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MetaTraderPlatforms", t => t.MetaTraderPlatformId, cascadeDelete: true)
                .Index(t => t.MetaTraderPlatformId);
            
            CreateTable(
                "dbo.MetaTraderPlatforms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SrvFilePath = c.String(maxLength: 2147483647),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StratDealingArbPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OpenTime = c.DateTime(nullable: false),
                        CloseTime = c.DateTime(),
                        AlphaOpenSignal = c.Decimal(nullable: false, precision: 18, scale: 5),
                        AlphaOpenPrice = c.Decimal(nullable: false, precision: 18, scale: 5),
                        AlphaClosePrice = c.Decimal(precision: 18, scale: 5),
                        AlphaCloseSignal = c.Decimal(precision: 18, scale: 5),
                        AlphaSide = c.Int(nullable: false),
                        AlphaSize = c.Decimal(nullable: false, precision: 18, scale: 3),
                        RemainingAlpha = c.Decimal(precision: 18, scale: 3),
                        AlphaOrderTicket = c.Long(),
                        BetaOpenSignal = c.Decimal(nullable: false, precision: 18, scale: 5),
                        BetaOpenPrice = c.Decimal(nullable: false, precision: 18, scale: 5),
                        BetaClosePrice = c.Decimal(precision: 18, scale: 5),
                        BetaCloseSignal = c.Decimal(precision: 18, scale: 5),
                        BetaSide = c.Int(nullable: false),
                        BetaSize = c.Decimal(nullable: false, precision: 18, scale: 3),
                        RemainingBeta = c.Decimal(precision: 18, scale: 3),
                        BetaOrderTicket = c.Long(),
                        IsClosed = c.Boolean(nullable: false),
                        StratDealingArbId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StratDealingArbs", t => t.StratDealingArbId, cascadeDelete: true)
                .Index(t => t.StratDealingArbId);
            
            CreateTable(
                "dbo.StratDealingArbs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Run = c.Boolean(nullable: false),
                        MaxNumberOfPositions = c.Int(nullable: false),
                        SignalDiffInPip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SignalStepInPip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TargetInPip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinOpenTimeInMinutes = c.Int(nullable: false),
                        ReOpenIntervalInMinutes = c.Int(nullable: false),
                        EarliestOpenTime = c.Int(),
                        LatestOpenTime = c.Int(),
                        LatestCloseTime = c.Int(),
                        OrderType = c.Int(nullable: false),
                        MaxRetryCount = c.Int(nullable: false),
                        RetryPeriodInMs = c.Int(nullable: false),
                        SlippageInPip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeWindowInMs = c.Int(nullable: false),
                        AlphaAccountId = c.Int(nullable: false),
                        AlphaSymbol = c.String(nullable: false, maxLength: 2147483647),
                        AlphaSize = c.Decimal(nullable: false, precision: 18, scale: 3),
                        BetaAccountId = c.Int(nullable: false),
                        BetaSymbol = c.String(nullable: false, maxLength: 2147483647),
                        BetaSize = c.Decimal(nullable: false, precision: 18, scale: 3),
                        ShiftInPip = c.Decimal(precision: 18, scale: 2),
                        ShiftCalcIntervalInMinutes = c.Int(nullable: false),
                        PipSize = c.Decimal(nullable: false, precision: 18, scale: 5),
                        MagicNumber = c.Int(nullable: false),
                        LastOpenTime = c.DateTime(),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AlphaAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.BetaAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.AlphaAccountId)
                .Index(t => t.BetaAccountId);
            
            CreateTable(
                "dbo.StratHubArbs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AggregatorId = c.Int(nullable: false),
                        Run = c.Boolean(nullable: false),
                        MaxNumberOfPositions = c.Int(nullable: false),
                        SignalDiffInPip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SignalStepInPip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TargetInPip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinOpenTimeInMinutes = c.Int(nullable: false),
                        ReOpenIntervalInMinutes = c.Int(nullable: false),
                        EarliestOpenTime = c.Int(),
                        LatestOpenTime = c.Int(),
                        LatestCloseTime = c.Int(),
                        OrderType = c.Int(nullable: false),
                        MaxRetryCount = c.Int(nullable: false),
                        RetryPeriodInMs = c.Int(nullable: false),
                        SlippageInPip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeWindowInMs = c.Int(nullable: false),
                        Size = c.Decimal(nullable: false, precision: 18, scale: 3),
                        PipSize = c.Decimal(nullable: false, precision: 18, scale: 5),
                        Description = c.String(nullable: false, maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Aggregators", t => t.AggregatorId, cascadeDelete: true)
                .Index(t => t.AggregatorId);
            
            CreateTable(
                "dbo.Tickers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        MainAccountId = c.Int(nullable: false),
                        MainSymbol = c.String(maxLength: 2147483647),
                        PairAccountId = c.Int(),
                        PairSymbol = c.String(maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.MainAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.PairAccountId)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.MainAccountId)
                .Index(t => t.PairAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickers", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Tickers", "PairAccountId", "dbo.Accounts");
            DropForeignKey("dbo.Tickers", "MainAccountId", "dbo.Accounts");
            DropForeignKey("dbo.StratHubArbs", "AggregatorId", "dbo.Aggregators");
            DropForeignKey("dbo.StratDealingArbs", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.StratDealingArbPositions", "StratDealingArbId", "dbo.StratDealingArbs");
            DropForeignKey("dbo.StratDealingArbs", "BetaAccountId", "dbo.Accounts");
            DropForeignKey("dbo.StratDealingArbs", "AlphaAccountId", "dbo.Accounts");
            DropForeignKey("dbo.MetaTraderAccounts", "MetaTraderPlatformId", "dbo.MetaTraderPlatforms");
            DropForeignKey("dbo.Accounts", "MetaTraderAccountId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Accounts", "IlyaFastFeedAccountId", "dbo.IlyaFastFeedAccounts");
            DropForeignKey("dbo.Accounts", "FixTraderAccountId", "dbo.FixTraderAccounts");
            DropForeignKey("dbo.Accounts", "FixApiAccountId", "dbo.FixApiAccounts");
            DropForeignKey("dbo.CTraderAccounts", "CTraderPlatformId", "dbo.CTraderPlatforms");
            DropForeignKey("dbo.Accounts", "CTraderAccountId", "dbo.CTraderAccounts");
            DropForeignKey("dbo.Accounts", "CqgClientApiAccountId", "dbo.CqgClientApiAccounts");
            DropForeignKey("dbo.Aggregators", "ProfileId", "dbo.Profiles");
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
            DropForeignKey("dbo.AggregatorAccounts", "Aggregator_Id", "dbo.Aggregators");
            DropForeignKey("dbo.AggregatorAccounts", "Account_Id", "dbo.Accounts");
            DropIndex("dbo.Tickers", new[] { "PairAccountId" });
            DropIndex("dbo.Tickers", new[] { "MainAccountId" });
            DropIndex("dbo.Tickers", new[] { "ProfileId" });
            DropIndex("dbo.StratHubArbs", new[] { "AggregatorId" });
            DropIndex("dbo.StratDealingArbs", new[] { "BetaAccountId" });
            DropIndex("dbo.StratDealingArbs", new[] { "AlphaAccountId" });
            DropIndex("dbo.StratDealingArbs", new[] { "ProfileId" });
            DropIndex("dbo.StratDealingArbPositions", new[] { "StratDealingArbId" });
            DropIndex("dbo.MetaTraderAccounts", new[] { "MetaTraderPlatformId" });
            DropIndex("dbo.CTraderAccounts", new[] { "CTraderPlatformId" });
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
            DropIndex("dbo.Aggregators", new[] { "ProfileId" });
            DropIndex("dbo.AggregatorAccounts", new[] { "Account_Id" });
            DropIndex("dbo.AggregatorAccounts", new[] { "Aggregator_Id" });
            DropIndex("dbo.Accounts", new[] { "CqgClientApiAccountId" });
            DropIndex("dbo.Accounts", new[] { "IlyaFastFeedAccountId" });
            DropIndex("dbo.Accounts", new[] { "FixApiAccountId" });
            DropIndex("dbo.Accounts", new[] { "FixTraderAccountId" });
            DropIndex("dbo.Accounts", new[] { "MetaTraderAccountId" });
            DropIndex("dbo.Accounts", new[] { "CTraderAccountId" });
            DropIndex("dbo.Accounts", new[] { "ProfileId" });
            DropTable("dbo.Tickers");
            DropTable("dbo.StratHubArbs");
            DropTable("dbo.StratDealingArbs");
            DropTable("dbo.StratDealingArbPositions");
            DropTable("dbo.MetaTraderPlatforms");
            DropTable("dbo.MetaTraderAccounts");
            DropTable("dbo.IlyaFastFeedAccounts");
            DropTable("dbo.FixTraderAccounts");
            DropTable("dbo.FixApiAccounts");
            DropTable("dbo.CTraderPlatforms");
            DropTable("dbo.CTraderAccounts");
            DropTable("dbo.CqgClientApiAccounts");
            DropTable("dbo.PushingDetails");
            DropTable("dbo.Pushings");
            DropTable("dbo.SymbolMappings");
            DropTable("dbo.FixApiCopiers");
            DropTable("dbo.Copiers");
            DropTable("dbo.Slaves");
            DropTable("dbo.Masters");
            DropTable("dbo.Profiles");
            DropTable("dbo.Aggregators");
            DropTable("dbo.AggregatorAccounts");
            DropTable("dbo.Accounts");
        }
    }
}
