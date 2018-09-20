namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbOrderType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StratDealingArbs", "OrderType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StratDealingArbs", "OrderType");
        }
    }
}
