namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbUpdate2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StratDealingArbs", "ShiftDiffSumInPip");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StratDealingArbs", "ShiftDiffSumInPip", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
