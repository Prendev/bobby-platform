namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFixApiAccountTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FixApiAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConfigPath = c.String(),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Accounts", "FixApiAccountId", c => c.Int());
            AlterColumn("dbo.MetaTraderPlatforms", "SrvFilePath", c => c.String());
            CreateIndex("dbo.Accounts", "FixApiAccountId");
            AddForeignKey("dbo.Accounts", "FixApiAccountId", "dbo.FixApiAccounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accounts", "FixApiAccountId", "dbo.FixApiAccounts");
            DropIndex("dbo.Accounts", new[] { "FixApiAccountId" });
            AlterColumn("dbo.MetaTraderPlatforms", "SrvFilePath", c => c.String(nullable: false));
            DropColumn("dbo.Accounts", "FixApiAccountId");
            DropTable("dbo.FixApiAccounts");
        }
    }
}
