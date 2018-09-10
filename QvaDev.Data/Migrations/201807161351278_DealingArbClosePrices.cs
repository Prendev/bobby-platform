namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbClosePrices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StratDealingArbPositions", "AlphaClosePrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.StratDealingArbPositions", "BetaClosePrice", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StratDealingArbPositions", "BetaClosePrice");
            DropColumn("dbo.StratDealingArbPositions", "AlphaClosePrice");
        }
    }
}
