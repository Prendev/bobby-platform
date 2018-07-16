namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RetryPeriodRename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Copiers", "RetryPeriodInMs", c => c.Int(nullable: false));
            AddColumn("dbo.FixApiCopiers", "RetryPeriodInMs", c => c.Int(nullable: false));
            AddColumn("dbo.PushingDetails", "RetryPeriodInMs", c => c.Int(nullable: false));
            AddColumn("dbo.StratDealingArbs", "RetryPeriodInMs", c => c.Int(nullable: false));
            DropColumn("dbo.Copiers", "RetryPeriodInMilliseconds");
            DropColumn("dbo.FixApiCopiers", "RetryPeriodInMilliseconds");
            DropColumn("dbo.PushingDetails", "RetryPeriodInMilliseconds");
            DropColumn("dbo.StratDealingArbs", "RetryPeriodInMilliseconds");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StratDealingArbs", "RetryPeriodInMilliseconds", c => c.Int(nullable: false));
            AddColumn("dbo.PushingDetails", "RetryPeriodInMilliseconds", c => c.Int(nullable: false));
            AddColumn("dbo.FixApiCopiers", "RetryPeriodInMilliseconds", c => c.Int(nullable: false));
            AddColumn("dbo.Copiers", "RetryPeriodInMilliseconds", c => c.Int(nullable: false));
            DropColumn("dbo.StratDealingArbs", "RetryPeriodInMs");
            DropColumn("dbo.PushingDetails", "RetryPeriodInMs");
            DropColumn("dbo.FixApiCopiers", "RetryPeriodInMs");
            DropColumn("dbo.Copiers", "RetryPeriodInMs");
        }
    }
}
