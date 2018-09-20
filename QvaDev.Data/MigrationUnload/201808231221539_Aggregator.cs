namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aggregator : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AggregatorAccounts",
                c => new
                    {
                        Aggregator_Id = c.Int(nullable: false),
                        Account_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Aggregator_Id, t.Account_Id })
                .ForeignKey("dbo.Accounts", t => t.Account_Id, cascadeDelete: true)
                .ForeignKey("dbo.Aggregators", t => t.Aggregator_Id, cascadeDelete: true)
                .Index(t => t.Aggregator_Id)
                .Index(t => t.Account_Id);
            
            CreateTable(
                "dbo.Aggregators",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: false)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Aggregators", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.AggregatorAccounts", "Aggregator_Id", "dbo.Aggregators");
            DropForeignKey("dbo.AggregatorAccounts", "Account_Id", "dbo.Accounts");
            DropIndex("dbo.Aggregators", new[] { "ProfileId" });
            DropIndex("dbo.AggregatorAccounts", new[] { "Account_Id" });
            DropIndex("dbo.AggregatorAccounts", new[] { "Aggregator_Id" });
            DropTable("dbo.Aggregators");
            DropTable("dbo.AggregatorAccounts");
        }
    }
}
