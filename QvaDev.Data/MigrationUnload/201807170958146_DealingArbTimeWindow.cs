namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbTimeWindow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StratDealingArbs", "TimeWindowInMs", c => c.Int(nullable: false));
            DropColumn("dbo.StratDealingArbs", "Ttl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StratDealingArbs", "Ttl", c => c.Int(nullable: false));
            DropColumn("dbo.StratDealingArbs", "TimeWindowInMs");
        }
    }
}
