namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealingArbUpdate : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.StratDealingArbs", name: "FtAccountId", newName: "AlphaAccountId");
            RenameColumn(table: "dbo.StratDealingArbs", name: "MtAccountId", newName: "BetaAccountId");
            RenameIndex(table: "dbo.StratDealingArbs", name: "IX_FtAccountId", newName: "IX_AlphaAccountId");
            RenameIndex(table: "dbo.StratDealingArbs", name: "IX_MtAccountId", newName: "IX_BetaAccountId");
            CreateTable(
                "dbo.StratDealingArbPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StratDealingArbId = c.Int(nullable: false),
                        OpenTime = c.DateTime(nullable: false),
                        AlphaOpenPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AlphaSide = c.Int(nullable: false),
                        AlphaSize = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AlphaOrderTicket = c.Long(),
                        BetaOpenPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BetaSide = c.Int(nullable: false),
                        BetaSize = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BetaOrderTicket = c.Long(),
                        IsClosed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StratDealingArbs", t => t.StratDealingArbId, cascadeDelete: true)
                .Index(t => t.StratDealingArbId);
            
            AddColumn("dbo.StratDealingArbs", "AlphaSymbol", c => c.String(nullable: false));
            AddColumn("dbo.StratDealingArbs", "AlphaSize", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.StratDealingArbs", "BetaSymbol", c => c.String(nullable: false));
            AddColumn("dbo.StratDealingArbs", "BetaSize", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.StratDealingArbs", "ShiftDiffSumInPip", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.StratDealingArbs", "ContractSize");
            DropColumn("dbo.StratDealingArbs", "Lots");
            DropColumn("dbo.StratDealingArbs", "FtSymbol");
            DropColumn("dbo.StratDealingArbs", "MtSymbol");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StratDealingArbs", "MtSymbol", c => c.String(nullable: false));
            AddColumn("dbo.StratDealingArbs", "FtSymbol", c => c.String(nullable: false));
            AddColumn("dbo.StratDealingArbs", "Lots", c => c.Double(nullable: false));
            AddColumn("dbo.StratDealingArbs", "ContractSize", c => c.Int(nullable: false));
            DropForeignKey("dbo.StratDealingArbPositions", "StratDealingArbId", "dbo.StratDealingArbs");
            DropIndex("dbo.StratDealingArbPositions", new[] { "StratDealingArbId" });
            DropColumn("dbo.StratDealingArbs", "ShiftDiffSumInPip");
            DropColumn("dbo.StratDealingArbs", "BetaSize");
            DropColumn("dbo.StratDealingArbs", "BetaSymbol");
            DropColumn("dbo.StratDealingArbs", "AlphaSize");
            DropColumn("dbo.StratDealingArbs", "AlphaSymbol");
            DropTable("dbo.StratDealingArbPositions");
            RenameIndex(table: "dbo.StratDealingArbs", name: "IX_BetaAccountId", newName: "IX_MtAccountId");
            RenameIndex(table: "dbo.StratDealingArbs", name: "IX_AlphaAccountId", newName: "IX_FtAccountId");
            RenameColumn(table: "dbo.StratDealingArbs", name: "BetaAccountId", newName: "MtAccountId");
            RenameColumn(table: "dbo.StratDealingArbs", name: "AlphaAccountId", newName: "FtAccountId");
        }
    }
}
