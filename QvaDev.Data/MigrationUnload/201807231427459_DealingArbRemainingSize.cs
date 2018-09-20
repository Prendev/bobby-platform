namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbRemainingSize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StratDealingArbPositions", "RemainingAlpha", c => c.Decimal(precision: 18, scale: 3));
            AddColumn("dbo.StratDealingArbPositions", "RemainingBeta", c => c.Decimal(precision: 18, scale: 3));
            AddColumn("dbo.StratDealingArbs", "Run", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StratDealingArbs", "Run");
            DropColumn("dbo.StratDealingArbPositions", "RemainingBeta");
            DropColumn("dbo.StratDealingArbPositions", "RemainingAlpha");
        }
    }
}
