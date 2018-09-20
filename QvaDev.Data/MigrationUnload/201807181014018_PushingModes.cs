namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PushingModes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pushings", "FutureExecutionMode", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pushings", "FutureExecutionMode");
        }
    }
}
