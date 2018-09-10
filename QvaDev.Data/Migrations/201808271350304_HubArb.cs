namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HubArb : DbMigration
    {
        public override void Up()
        {
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
                        EarliestOpenTime = c.Time(precision: 7),
                        LatestOpenTime = c.Time(precision: 7),
                        LatestCloseTime = c.Time(precision: 7),
                        OrderType = c.Int(nullable: false),
                        MaxRetryCount = c.Int(nullable: false),
                        RetryPeriodInMs = c.Int(nullable: false),
                        SlippageInPip = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeWindowInMs = c.Int(nullable: false),
                        Size = c.Decimal(nullable: false, precision: 18, scale: 3),
                        PipSize = c.Decimal(nullable: false, precision: 18, scale: 5),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Aggregators", t => t.AggregatorId, cascadeDelete: true)
                .Index(t => t.AggregatorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StratHubArbs", "AggregatorId", "dbo.Aggregators");
            DropIndex("dbo.StratHubArbs", new[] { "AggregatorId" });
            DropTable("dbo.StratHubArbs");
        }
    }
}
