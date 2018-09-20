namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IlyaFastFeedAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IlyaFastFeedAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(),
                        Port = c.Int(nullable: false),
                        UserName = c.String(),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Accounts", "IlyaFastFeedAccountId", c => c.Int());
            CreateIndex("dbo.Accounts", "IlyaFastFeedAccountId");
            AddForeignKey("dbo.Accounts", "IlyaFastFeedAccountId", "dbo.IlyaFastFeedAccounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accounts", "IlyaFastFeedAccountId", "dbo.IlyaFastFeedAccounts");
            DropIndex("dbo.Accounts", new[] { "IlyaFastFeedAccountId" });
            DropColumn("dbo.Accounts", "IlyaFastFeedAccountId");
            DropTable("dbo.IlyaFastFeedAccounts");
        }
    }
}
