namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Copiers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SlaveId = c.Int(nullable: false),
                        CopyRatio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UseMarketRangeOrder = c.Boolean(nullable: false),
                        SlippageInPips = c.Int(nullable: false),
                        MaxRetryCount = c.Int(nullable: false),
                        RetryPeriodInMilliseconds = c.Int(nullable: false),
                        DelayInMilliseconds = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Slaves", t => t.SlaveId, cascadeDelete: true)
                .Index(t => t.SlaveId);
            
            CreateTable(
                "dbo.Slaves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MasterId = c.Int(nullable: false),
                        CTraderAccountId = c.Int(),
                        MetaTraderAccountId = c.Int(),
                        SymbolSuffix = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CTraderAccounts", t => t.CTraderAccountId)
                .ForeignKey("dbo.Masters", t => t.MasterId, cascadeDelete: true)
                .ForeignKey("dbo.MetaTraderAccounts", t => t.MetaTraderAccountId)
                .Index(t => t.MasterId)
                .Index(t => t.CTraderAccountId)
                .Index(t => t.MetaTraderAccountId);
            
            CreateTable(
                "dbo.CTraderAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.Long(nullable: false),
                        AccessToken = c.String(nullable: false),
                        CTraderPlatformId = c.Int(nullable: false),
                        ShouldConnect = c.Boolean(nullable: false),
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
                "dbo.MonitoredAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MonitorId = c.Int(nullable: false),
                        IsMaster = c.Boolean(nullable: false),
                        CTraderAccountId = c.Int(),
                        MetaTraderAccountId = c.Int(),
                        Symbol = c.String(),
                        ExpectedContracts = c.Long(nullable: false),
                        FixTraderAccount_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CTraderAccounts", t => t.CTraderAccountId)
                .ForeignKey("dbo.MetaTraderAccounts", t => t.MetaTraderAccountId)
                .ForeignKey("dbo.FixTraderAccounts", t => t.FixTraderAccount_Id)
                .ForeignKey("dbo.Monitors", t => t.MonitorId, cascadeDelete: true)
                .Index(t => t.MonitorId)
                .Index(t => t.CTraderAccountId)
                .Index(t => t.MetaTraderAccountId)
                .Index(t => t.FixTraderAccount_Id);
            
            CreateTable(
                "dbo.MetaTraderAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User = c.Long(nullable: false),
                        Password = c.String(nullable: false),
                        MetaTraderPlatformId = c.Int(nullable: false),
                        ShouldConnect = c.Boolean(nullable: false),
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
                "dbo.Monitors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Symbol = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            CreateTable(
                "dbo.Masters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        MetaTraderAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.MetaTraderAccounts", t => t.MetaTraderAccountId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.MetaTraderAccountId);
            
            CreateTable(
                "dbo.Pushings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        FutureAccountId = c.Int(nullable: false),
                        FutureSymbol = c.String(nullable: false),
                        AlphaMasterId = c.Int(),
                        AlphaSymbol = c.String(),
                        BetaMasterId = c.Int(),
                        BetaSymbol = c.String(),
                        HedgeAccountId = c.Int(),
                        HedgeSymbol = c.String(),
                        PushingDetailId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MetaTraderAccounts", t => t.AlphaMasterId)
                .ForeignKey("dbo.MetaTraderAccounts", t => t.BetaMasterId)
                .ForeignKey("dbo.FixTraderAccounts", t => t.FutureAccountId, cascadeDelete: true)
                .ForeignKey("dbo.MetaTraderAccounts", t => t.HedgeAccountId)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .ForeignKey("dbo.PushingDetails", t => t.PushingDetailId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.FutureAccountId)
                .Index(t => t.AlphaMasterId)
                .Index(t => t.BetaMasterId)
                .Index(t => t.HedgeAccountId)
                .Index(t => t.PushingDetailId);
            
            CreateTable(
                "dbo.FixTraderAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(nullable: false),
                        CommandSocketPort = c.Int(nullable: false),
                        EventsSocketPort = c.Int(nullable: false),
                        ShouldConnect = c.Boolean(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        MasterLots = c.Double(nullable: false),
                        HedgeLots = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TradingAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShouldTrade = c.Boolean(nullable: false),
                        ProfileId = c.Int(nullable: false),
                        ExpertId = c.Int(nullable: false),
                        MetaTraderAccountId = c.Int(nullable: false),
                        TradeSetFloatingSwitch = c.Double(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Experts", t => t.ExpertId, cascadeDelete: true)
                .ForeignKey("dbo.MetaTraderAccounts", t => t.MetaTraderAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.ExpertId)
                .Index(t => t.MetaTraderAccountId);
            
            CreateTable(
                "dbo.Experts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuadroSets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShouldRun = c.Boolean(nullable: false),
                        TradeOpeningEnabled = c.Boolean(nullable: false),
                        TimeFrame = c.Int(nullable: false),
                        Symbol1 = c.String(),
                        Symbol2 = c.String(),
                        Variant = c.Int(nullable: false),
                        Diff = c.Int(nullable: false),
                        Period = c.Int(nullable: false),
                        LotSize = c.Double(nullable: false),
                        Tp1 = c.Int(nullable: false),
                        ReOpenDiff = c.Int(nullable: false),
                        ReOpenDiffChangeCount = c.Int(nullable: false),
                        ReOpenDiff2 = c.Int(nullable: false),
                        Delta = c.Int(nullable: false),
                        M = c.Double(nullable: false),
                        MagicNumber = c.Int(nullable: false),
                        MaxTradeSetCount = c.Int(nullable: false),
                        Last24HMaxOpen = c.Int(nullable: false),
                        ProfitCloseBuy = c.Boolean(nullable: false),
                        ProfitCloseValueBuy = c.Double(nullable: false),
                        ProfitCloseSell = c.Boolean(nullable: false),
                        ProfitCloseValueSell = c.Double(nullable: false),
                        CurrentBuyState = c.Int(nullable: false),
                        BuyOpenCount = c.Int(nullable: false),
                        Sym1LastMinActionPrice = c.Double(nullable: false),
                        Sym2LastMinActionPrice = c.Double(nullable: false),
                        CurrentSellState = c.Int(nullable: false),
                        SellOpenCount = c.Int(nullable: false),
                        Sym1LastMaxActionPrice = c.Double(nullable: false),
                        Sym2LastMaxActionPrice = c.Double(nullable: false),
                        TradingAccountId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TradingAccounts", t => t.TradingAccountId, cascadeDelete: true)
                .Index(t => t.TradingAccountId);
            
            CreateTable(
                "dbo.SymbolMappings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SlaveId = c.Int(nullable: false),
                        From = c.String(nullable: false),
                        To = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Slaves", t => t.SlaveId, cascadeDelete: true)
                .Index(t => t.SlaveId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Copiers", "SlaveId", "dbo.Slaves");
            DropForeignKey("dbo.SymbolMappings", "SlaveId", "dbo.Slaves");
            DropForeignKey("dbo.Slaves", "MetaTraderAccountId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Slaves", "MasterId", "dbo.Masters");
            DropForeignKey("dbo.Slaves", "CTraderAccountId", "dbo.CTraderAccounts");
            DropForeignKey("dbo.MonitoredAccounts", "MonitorId", "dbo.Monitors");
            DropForeignKey("dbo.Monitors", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.QuadroSets", "TradingAccountId", "dbo.TradingAccounts");
            DropForeignKey("dbo.TradingAccounts", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.TradingAccounts", "MetaTraderAccountId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.TradingAccounts", "ExpertId", "dbo.Experts");
            DropForeignKey("dbo.Pushings", "PushingDetailId", "dbo.PushingDetails");
            DropForeignKey("dbo.Pushings", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Pushings", "HedgeAccountId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Pushings", "FutureAccountId", "dbo.FixTraderAccounts");
            DropForeignKey("dbo.MonitoredAccounts", "FixTraderAccount_Id", "dbo.FixTraderAccounts");
            DropForeignKey("dbo.Pushings", "BetaMasterId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Pushings", "AlphaMasterId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Groups", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Masters", "MetaTraderAccountId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Masters", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.MonitoredAccounts", "MetaTraderAccountId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.MetaTraderAccounts", "MetaTraderPlatformId", "dbo.MetaTraderPlatforms");
            DropForeignKey("dbo.MonitoredAccounts", "CTraderAccountId", "dbo.CTraderAccounts");
            DropForeignKey("dbo.CTraderAccounts", "CTraderPlatformId", "dbo.CTraderPlatforms");
            DropIndex("dbo.SymbolMappings", new[] { "SlaveId" });
            DropIndex("dbo.QuadroSets", new[] { "TradingAccountId" });
            DropIndex("dbo.TradingAccounts", new[] { "MetaTraderAccountId" });
            DropIndex("dbo.TradingAccounts", new[] { "ExpertId" });
            DropIndex("dbo.TradingAccounts", new[] { "ProfileId" });
            DropIndex("dbo.Pushings", new[] { "PushingDetailId" });
            DropIndex("dbo.Pushings", new[] { "HedgeAccountId" });
            DropIndex("dbo.Pushings", new[] { "BetaMasterId" });
            DropIndex("dbo.Pushings", new[] { "AlphaMasterId" });
            DropIndex("dbo.Pushings", new[] { "FutureAccountId" });
            DropIndex("dbo.Pushings", new[] { "ProfileId" });
            DropIndex("dbo.Masters", new[] { "MetaTraderAccountId" });
            DropIndex("dbo.Masters", new[] { "GroupId" });
            DropIndex("dbo.Groups", new[] { "ProfileId" });
            DropIndex("dbo.Monitors", new[] { "ProfileId" });
            DropIndex("dbo.MetaTraderAccounts", new[] { "MetaTraderPlatformId" });
            DropIndex("dbo.MonitoredAccounts", new[] { "FixTraderAccount_Id" });
            DropIndex("dbo.MonitoredAccounts", new[] { "MetaTraderAccountId" });
            DropIndex("dbo.MonitoredAccounts", new[] { "CTraderAccountId" });
            DropIndex("dbo.MonitoredAccounts", new[] { "MonitorId" });
            DropIndex("dbo.CTraderAccounts", new[] { "CTraderPlatformId" });
            DropIndex("dbo.Slaves", new[] { "MetaTraderAccountId" });
            DropIndex("dbo.Slaves", new[] { "CTraderAccountId" });
            DropIndex("dbo.Slaves", new[] { "MasterId" });
            DropIndex("dbo.Copiers", new[] { "SlaveId" });
            DropTable("dbo.SymbolMappings");
            DropTable("dbo.QuadroSets");
            DropTable("dbo.Experts");
            DropTable("dbo.TradingAccounts");
            DropTable("dbo.PushingDetails");
            DropTable("dbo.FixTraderAccounts");
            DropTable("dbo.Pushings");
            DropTable("dbo.Masters");
            DropTable("dbo.Groups");
            DropTable("dbo.Profiles");
            DropTable("dbo.Monitors");
            DropTable("dbo.MetaTraderPlatforms");
            DropTable("dbo.MetaTraderAccounts");
            DropTable("dbo.MonitoredAccounts");
            DropTable("dbo.CTraderPlatforms");
            DropTable("dbo.CTraderAccounts");
            DropTable("dbo.Slaves");
            DropTable("dbo.Copiers");
        }
    }
}
