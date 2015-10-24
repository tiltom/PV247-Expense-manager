namespace ExpenseManager.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMissingRequiredFields : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Budget", new[] { "Creator_Id" });
            DropIndex("dbo.Budget", new[] { "Currency_Guid" });
            DropIndex("dbo.WalletAccessRight", new[] { "User_Id" });
            DropIndex("dbo.WalletAccessRight", new[] { "Wallet_Guid" });
            AlterColumn("dbo.Budget", "Creator_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Budget", "Currency_Guid", c => c.Guid(nullable: false));
            AlterColumn("dbo.WalletAccessRight", "User_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.WalletAccessRight", "Wallet_Guid", c => c.Guid(nullable: false));
            CreateIndex("dbo.Budget", "Creator_Id");
            CreateIndex("dbo.Budget", "Currency_Guid");
            CreateIndex("dbo.WalletAccessRight", "User_Id");
            CreateIndex("dbo.WalletAccessRight", "Wallet_Guid");
        }
        
        public override void Down()
        {
            DropIndex("dbo.WalletAccessRight", new[] { "Wallet_Guid" });
            DropIndex("dbo.WalletAccessRight", new[] { "User_Id" });
            DropIndex("dbo.Budget", new[] { "Currency_Guid" });
            DropIndex("dbo.Budget", new[] { "Creator_Id" });
            AlterColumn("dbo.WalletAccessRight", "Wallet_Guid", c => c.Guid());
            AlterColumn("dbo.WalletAccessRight", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Budget", "Currency_Guid", c => c.Guid());
            AlterColumn("dbo.Budget", "Creator_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.WalletAccessRight", "Wallet_Guid");
            CreateIndex("dbo.WalletAccessRight", "User_Id");
            CreateIndex("dbo.Budget", "Currency_Guid");
            CreateIndex("dbo.Budget", "Creator_Id");
        }
    }
}
