namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbCloseTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StratDealingArbPositions", "CloseTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StratDealingArbPositions", "CloseTime");
        }
    }
}
