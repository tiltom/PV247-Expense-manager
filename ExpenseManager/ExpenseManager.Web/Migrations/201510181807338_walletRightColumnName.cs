namespace ExpenseManager.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class walletRightColumnName : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.WalletAccessRight", name: "Budget_Guid", newName: "Wallet_Guid");
            RenameIndex(table: "dbo.WalletAccessRight", name: "IX_Budget_Guid", newName: "IX_Wallet_Guid");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.WalletAccessRight", name: "IX_Wallet_Guid", newName: "IX_Budget_Guid");
            RenameColumn(table: "dbo.WalletAccessRight", name: "Wallet_Guid", newName: "Budget_Guid");
        }
    }
}
