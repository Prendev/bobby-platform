namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbSignalPrices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StratDealingArbPositions", "AlphaOpenSignal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.StratDealingArbPositions", "AlphaCloseSignal", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.StratDealingArbPositions", "BetaOpenSignal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.StratDealingArbPositions", "BetaCloseSignal", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StratDealingArbPositions", "BetaCloseSignal");
            DropColumn("dbo.StratDealingArbPositions", "BetaOpenSignal");
            DropColumn("dbo.StratDealingArbPositions", "AlphaCloseSignal");
            DropColumn("dbo.StratDealingArbPositions", "AlphaOpenSignal");
        }
    }
}
