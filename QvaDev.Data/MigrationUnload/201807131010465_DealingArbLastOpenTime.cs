namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbLastOpenTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StratDealingArbs", "LastOpenTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StratDealingArbs", "LastOpenTime");
        }
    }
}
