namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AggregatorSymbol : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AggregatorAccounts", "Symbol", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AggregatorAccounts", "Symbol");
        }
    }
}
