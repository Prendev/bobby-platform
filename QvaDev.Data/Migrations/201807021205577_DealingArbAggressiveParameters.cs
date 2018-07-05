namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbAggressiveParameters : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StratDealingArbs", "Deviation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.StratDealingArbs", "Ttl", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StratDealingArbs", "Ttl");
            DropColumn("dbo.StratDealingArbs", "Deviation");
        }
    }
}
