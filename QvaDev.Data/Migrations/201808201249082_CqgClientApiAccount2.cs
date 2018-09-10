namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CqgClientApiAccount2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Accounts", name: "CqgClientApiAccount_Id", newName: "CqgClientApiAccountId");
            RenameIndex(table: "dbo.Accounts", name: "IX_CqgClientApiAccount_Id", newName: "IX_CqgClientApiAccountId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Accounts", name: "IX_CqgClientApiAccountId", newName: "IX_CqgClientApiAccount_Id");
            RenameColumn(table: "dbo.Accounts", name: "CqgClientApiAccountId", newName: "CqgClientApiAccount_Id");
        }
    }
}
