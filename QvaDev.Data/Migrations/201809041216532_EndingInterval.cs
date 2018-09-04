namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EndingInterval : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PushingDetails", "EndingMinIntervalInMs", c => c.Int(nullable: false));
            AddColumn("dbo.PushingDetails", "EndingMaxIntervalInMs", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PushingDetails", "EndingMaxIntervalInMs");
            DropColumn("dbo.PushingDetails", "EndingMinIntervalInMs");
        }
    }
}
