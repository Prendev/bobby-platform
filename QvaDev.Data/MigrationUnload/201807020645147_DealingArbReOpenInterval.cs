namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbReOpenInterval : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StratDealingArbs", "ReOpenIntervalInMinutes", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StratDealingArbs", "ReOpenIntervalInMinutes");
        }
    }
}
