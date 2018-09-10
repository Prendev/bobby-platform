namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartialHedgeClosePercentage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PushingDetails", "PartialHedgeClosePercentage", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PushingDetails", "PartialHedgeClosePercentage");
        }
    }
}
