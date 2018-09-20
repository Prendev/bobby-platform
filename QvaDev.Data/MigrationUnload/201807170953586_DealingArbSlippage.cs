namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbSlippage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StratDealingArbs", "SlippageInPip", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.StratDealingArbs", "Deviation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StratDealingArbs", "Deviation", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            DropColumn("dbo.StratDealingArbs", "SlippageInPip");
        }
    }
}
