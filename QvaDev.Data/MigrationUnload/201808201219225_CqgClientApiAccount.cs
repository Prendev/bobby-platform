namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CqgClientApiAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CqgClientApiAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Accounts", "CqgClientApiAccount_Id", c => c.Int());
            CreateIndex("dbo.Accounts", "CqgClientApiAccount_Id");
            AddForeignKey("dbo.Accounts", "CqgClientApiAccount_Id", "dbo.CqgClientApiAccounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accounts", "CqgClientApiAccount_Id", "dbo.CqgClientApiAccounts");
            DropIndex("dbo.Accounts", new[] { "CqgClientApiAccount_Id" });
            DropColumn("dbo.Accounts", "CqgClientApiAccount_Id");
            DropTable("dbo.CqgClientApiAccounts");
        }
    }
}
