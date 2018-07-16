namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbSignalPricesFix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.StratDealingArbPositions", "AlphaOpenSignal", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.StratDealingArbPositions", "AlphaClosePrice", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.StratDealingArbPositions", "AlphaCloseSignal", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.StratDealingArbPositions", "BetaOpenSignal", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.StratDealingArbPositions", "BetaClosePrice", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.StratDealingArbPositions", "BetaCloseSignal", c => c.Decimal(precision: 18, scale: 5));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.StratDealingArbPositions", "BetaCloseSignal", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbPositions", "BetaClosePrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbPositions", "BetaOpenSignal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbPositions", "AlphaCloseSignal", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbPositions", "AlphaClosePrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbPositions", "AlphaOpenSignal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
