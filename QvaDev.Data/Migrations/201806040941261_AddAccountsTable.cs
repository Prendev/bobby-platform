namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountsTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Slaves", "CTraderAccountId", "dbo.CTraderAccounts");
            DropForeignKey("dbo.Slaves", "FixTraderAccountId", "dbo.FixTraderAccounts");
            DropForeignKey("dbo.Slaves", "MetaTraderAccountId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Tickers", "MainFixTraderAccountId", "dbo.FixTraderAccounts");
            DropForeignKey("dbo.Tickers", "MainMetaTraderAccountId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Tickers", "PairFixTraderAccountId", "dbo.FixTraderAccounts");
            DropForeignKey("dbo.Tickers", "PairMetaTraderAccountId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Pushings", "AlphaMasterId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Pushings", "BetaMasterId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Pushings", "HedgeAccountId", "dbo.MetaTraderAccounts");
            DropIndex("dbo.Slaves", new[] { "CTraderAccountId" });
            DropIndex("dbo.Slaves", new[] { "MetaTraderAccountId" });
            DropIndex("dbo.Slaves", new[] { "FixTraderAccountId" });
            DropIndex("dbo.Pushings", new[] { "AlphaMasterId" });
            DropIndex("dbo.Pushings", new[] { "BetaMasterId" });
            DropIndex("dbo.Pushings", new[] { "HedgeAccountId" });
            DropIndex("dbo.Tickers", new[] { "MainMetaTraderAccountId" });
            DropIndex("dbo.Tickers", new[] { "MainFixTraderAccountId" });
            DropIndex("dbo.Tickers", new[] { "PairMetaTraderAccountId" });
            DropIndex("dbo.Tickers", new[] { "PairFixTraderAccountId" });
            RenameColumn(table: "dbo.MonitoredAccounts", name: "CTraderAccountId", newName: "CTraderAccount_Id");
            RenameColumn(table: "dbo.MonitoredAccounts", name: "MetaTraderAccountId", newName: "MetaTraderAccount_Id");
            RenameIndex(table: "dbo.MonitoredAccounts", name: "IX_MetaTraderAccountId", newName: "IX_MetaTraderAccount_Id");
            RenameIndex(table: "dbo.MonitoredAccounts", name: "IX_CTraderAccountId", newName: "IX_CTraderAccount_Id");
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Run = c.Boolean(nullable: false),
                        CTraderAccountId = c.Int(),
                        MetaTraderAccountId = c.Int(),
                        FixTraderAccountId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CTraderAccounts", t => t.CTraderAccountId)
                .ForeignKey("dbo.FixTraderAccounts", t => t.FixTraderAccountId)
                .ForeignKey("dbo.MetaTraderAccounts", t => t.MetaTraderAccountId)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: false)
                .Index(t => t.ProfileId)
                .Index(t => t.CTraderAccountId)
                .Index(t => t.MetaTraderAccountId)
                .Index(t => t.FixTraderAccountId);
            
            AddColumn("dbo.Slaves", "AccountId", c => c.Int(nullable: false));
            AddColumn("dbo.MonitoredAccounts", "AccountId", c => c.Int(nullable: false));
            AddColumn("dbo.Tickers", "MainAccountId", c => c.Int(nullable: false));
            AddColumn("dbo.Tickers", "PairAccountId", c => c.Int());
            AlterColumn("dbo.Pushings", "AlphaMasterId", c => c.Int(nullable: false));
            AlterColumn("dbo.Pushings", "AlphaSymbol", c => c.String(nullable: false));
            AlterColumn("dbo.Pushings", "BetaMasterId", c => c.Int(nullable: false));
            AlterColumn("dbo.Pushings", "BetaSymbol", c => c.String(nullable: false));
            AlterColumn("dbo.Pushings", "HedgeAccountId", c => c.Int(nullable: false));
            AlterColumn("dbo.Pushings", "HedgeSymbol", c => c.String(nullable: false));
            CreateIndex("dbo.MonitoredAccounts", "AccountId");
            CreateIndex("dbo.Pushings", "AlphaMasterId");
            CreateIndex("dbo.Pushings", "BetaMasterId");
            CreateIndex("dbo.Pushings", "HedgeAccountId");
            CreateIndex("dbo.Slaves", "AccountId");
            CreateIndex("dbo.Tickers", "MainAccountId");
            CreateIndex("dbo.Tickers", "PairAccountId");
            AddForeignKey("dbo.MonitoredAccounts", "AccountId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Slaves", "AccountId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tickers", "MainAccountId", "dbo.Accounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tickers", "PairAccountId", "dbo.Accounts", "Id");
            AddForeignKey("dbo.Pushings", "AlphaMasterId", "dbo.Accounts", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Pushings", "BetaMasterId", "dbo.Accounts", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Pushings", "HedgeAccountId", "dbo.Accounts", "Id", cascadeDelete: false);
            DropColumn("dbo.Slaves", "CTraderAccountId");
            DropColumn("dbo.Slaves", "MetaTraderAccountId");
            DropColumn("dbo.Slaves", "FixTraderAccountId");
            DropColumn("dbo.CTraderAccounts", "Run");
            DropColumn("dbo.MetaTraderAccounts", "Run");
            DropColumn("dbo.FixTraderAccounts", "Run");
            DropColumn("dbo.Tickers", "MainMetaTraderAccountId");
            DropColumn("dbo.Tickers", "MainFixTraderAccountId");
            DropColumn("dbo.Tickers", "PairMetaTraderAccountId");
            DropColumn("dbo.Tickers", "PairFixTraderAccountId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickers", "PairFixTraderAccountId", c => c.Int());
            AddColumn("dbo.Tickers", "PairMetaTraderAccountId", c => c.Int());
            AddColumn("dbo.Tickers", "MainFixTraderAccountId", c => c.Int());
            AddColumn("dbo.Tickers", "MainMetaTraderAccountId", c => c.Int());
            AddColumn("dbo.FixTraderAccounts", "Run", c => c.Boolean(nullable: false));
            AddColumn("dbo.MetaTraderAccounts", "Run", c => c.Boolean(nullable: false));
            AddColumn("dbo.CTraderAccounts", "Run", c => c.Boolean(nullable: false));
            AddColumn("dbo.Slaves", "FixTraderAccountId", c => c.Int());
            AddColumn("dbo.Slaves", "MetaTraderAccountId", c => c.Int());
            AddColumn("dbo.Slaves", "CTraderAccountId", c => c.Int());
            DropForeignKey("dbo.Pushings", "HedgeAccountId", "dbo.Accounts");
            DropForeignKey("dbo.Pushings", "BetaMasterId", "dbo.Accounts");
            DropForeignKey("dbo.Pushings", "AlphaMasterId", "dbo.Accounts");
            DropForeignKey("dbo.Tickers", "PairAccountId", "dbo.Accounts");
            DropForeignKey("dbo.Tickers", "MainAccountId", "dbo.Accounts");
            DropForeignKey("dbo.Slaves", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Accounts", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Accounts", "MetaTraderAccountId", "dbo.MetaTraderAccounts");
            DropForeignKey("dbo.Accounts", "FixTraderAccountId", "dbo.FixTraderAccounts");
            DropForeignKey("dbo.Accounts", "CTraderAccountId", "dbo.CTraderAccounts");
            DropForeignKey("dbo.MonitoredAccounts", "AccountId", "dbo.Accounts");
            DropIndex("dbo.Tickers", new[] { "PairAccountId" });
            DropIndex("dbo.Tickers", new[] { "MainAccountId" });
            DropIndex("dbo.Slaves", new[] { "AccountId" });
            DropIndex("dbo.Pushings", new[] { "HedgeAccountId" });
            DropIndex("dbo.Pushings", new[] { "BetaMasterId" });
            DropIndex("dbo.Pushings", new[] { "AlphaMasterId" });
            DropIndex("dbo.MonitoredAccounts", new[] { "AccountId" });
            DropIndex("dbo.Accounts", new[] { "FixTraderAccountId" });
            DropIndex("dbo.Accounts", new[] { "MetaTraderAccountId" });
            DropIndex("dbo.Accounts", new[] { "CTraderAccountId" });
            DropIndex("dbo.Accounts", new[] { "ProfileId" });
            AlterColumn("dbo.Pushings", "HedgeSymbol", c => c.String());
            AlterColumn("dbo.Pushings", "HedgeAccountId", c => c.Int());
            AlterColumn("dbo.Pushings", "BetaSymbol", c => c.String());
            AlterColumn("dbo.Pushings", "BetaMasterId", c => c.Int());
            AlterColumn("dbo.Pushings", "AlphaSymbol", c => c.String());
            AlterColumn("dbo.Pushings", "AlphaMasterId", c => c.Int());
            DropColumn("dbo.Tickers", "PairAccountId");
            DropColumn("dbo.Tickers", "MainAccountId");
            DropColumn("dbo.MonitoredAccounts", "AccountId");
            DropColumn("dbo.Slaves", "AccountId");
            DropTable("dbo.Accounts");
            RenameIndex(table: "dbo.MonitoredAccounts", name: "IX_CTraderAccount_Id", newName: "IX_CTraderAccountId");
            RenameIndex(table: "dbo.MonitoredAccounts", name: "IX_MetaTraderAccount_Id", newName: "IX_MetaTraderAccountId");
            RenameColumn(table: "dbo.MonitoredAccounts", name: "MetaTraderAccount_Id", newName: "MetaTraderAccountId");
            RenameColumn(table: "dbo.MonitoredAccounts", name: "CTraderAccount_Id", newName: "CTraderAccountId");
            CreateIndex("dbo.Tickers", "PairFixTraderAccountId");
            CreateIndex("dbo.Tickers", "PairMetaTraderAccountId");
            CreateIndex("dbo.Tickers", "MainFixTraderAccountId");
            CreateIndex("dbo.Tickers", "MainMetaTraderAccountId");
            CreateIndex("dbo.Pushings", "HedgeAccountId");
            CreateIndex("dbo.Pushings", "BetaMasterId");
            CreateIndex("dbo.Pushings", "AlphaMasterId");
            CreateIndex("dbo.Slaves", "FixTraderAccountId");
            CreateIndex("dbo.Slaves", "MetaTraderAccountId");
            CreateIndex("dbo.Slaves", "CTraderAccountId");
            AddForeignKey("dbo.Pushings", "HedgeAccountId", "dbo.MetaTraderAccounts", "Id");
            AddForeignKey("dbo.Pushings", "BetaMasterId", "dbo.MetaTraderAccounts", "Id");
            AddForeignKey("dbo.Pushings", "AlphaMasterId", "dbo.MetaTraderAccounts", "Id");
            AddForeignKey("dbo.Tickers", "PairMetaTraderAccountId", "dbo.MetaTraderAccounts", "Id");
            AddForeignKey("dbo.Tickers", "PairFixTraderAccountId", "dbo.FixTraderAccounts", "Id");
            AddForeignKey("dbo.Tickers", "MainMetaTraderAccountId", "dbo.MetaTraderAccounts", "Id");
            AddForeignKey("dbo.Tickers", "MainFixTraderAccountId", "dbo.FixTraderAccounts", "Id");
            AddForeignKey("dbo.Slaves", "MetaTraderAccountId", "dbo.MetaTraderAccounts", "Id");
            AddForeignKey("dbo.Slaves", "FixTraderAccountId", "dbo.FixTraderAccounts", "Id");
            AddForeignKey("dbo.Slaves", "CTraderAccountId", "dbo.CTraderAccounts", "Id");
        }
    }
}
