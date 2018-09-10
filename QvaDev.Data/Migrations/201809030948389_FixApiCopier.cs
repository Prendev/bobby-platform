namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixApiCopier : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FixApiCopiers", "SlippageInPip", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.FixApiCopiers", "TimeWindowInMs", c => c.Int(nullable: false));
            AddColumn("dbo.FixApiCopiers", "PipSize", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("dbo.PushingDetails", "PartialClosePercentage", c => c.Int(nullable: false));
            DropColumn("dbo.FixApiCopiers", "Slippage");
            DropColumn("dbo.FixApiCopiers", "BurstPeriodInMilliseconds");
            DropColumn("dbo.PushingDetails", "PartialHedgeClosePercentage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PushingDetails", "PartialHedgeClosePercentage", c => c.Int(nullable: false));
            AddColumn("dbo.FixApiCopiers", "BurstPeriodInMilliseconds", c => c.Int(nullable: false));
            AddColumn("dbo.FixApiCopiers", "Slippage", c => c.Double(nullable: false));
            DropColumn("dbo.PushingDetails", "PartialClosePercentage");
            DropColumn("dbo.FixApiCopiers", "PipSize");
            DropColumn("dbo.FixApiCopiers", "TimeWindowInMs");
            DropColumn("dbo.FixApiCopiers", "SlippageInPip");
        }
    }
}
