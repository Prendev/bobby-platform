namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DecimalFix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.StratDealingArbs", "EarliestOpenTime", c => c.Time(precision: 7));
            AlterColumn("dbo.StratDealingArbs", "LatestOpenTime", c => c.Time(precision: 7));
            AlterColumn("dbo.StratDealingArbs", "LatestCloseTime", c => c.Time(precision: 7));
            AlterColumn("dbo.StratDealingArbs", "Deviation", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.StratDealingArbs", "AlphaSize", c => c.Decimal(nullable: false, precision: 18, scale: 3));
            AlterColumn("dbo.StratDealingArbs", "BetaSize", c => c.Decimal(nullable: false, precision: 18, scale: 3));
            AlterColumn("dbo.StratDealingArbs", "PipSize", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.StratDealingArbPositions", "AlphaOpenPrice", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.StratDealingArbPositions", "AlphaSize", c => c.Decimal(nullable: false, precision: 18, scale: 3));
            AlterColumn("dbo.StratDealingArbPositions", "BetaOpenPrice", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.StratDealingArbPositions", "BetaSize", c => c.Decimal(nullable: false, precision: 18, scale: 3));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.StratDealingArbPositions", "BetaSize", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbPositions", "BetaOpenPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbPositions", "AlphaSize", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbPositions", "AlphaOpenPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbs", "PipSize", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbs", "BetaSize", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbs", "AlphaSize", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbs", "Deviation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbs", "LatestCloseTime", c => c.Time(nullable: false, precision: 7));
            AlterColumn("dbo.StratDealingArbs", "LatestOpenTime", c => c.Time(nullable: false, precision: 7));
            AlterColumn("dbo.StratDealingArbs", "EarliestOpenTime", c => c.Time(nullable: false, precision: 7));
        }
    }
}
