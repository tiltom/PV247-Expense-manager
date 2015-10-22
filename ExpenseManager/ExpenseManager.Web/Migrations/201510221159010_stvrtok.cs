namespace ExpenseManager.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stvrtok : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.BudgetAccessRight", new[] { "User_Id" });
            DropIndex("dbo.Budget", new[] { "Currency_Guid" });
            DropIndex("dbo.Wallet", new[] { "Owner_Id" });
            DropIndex("dbo.WalletAccessRight", new[] { "Wallet_Guid" });
            RenameColumn(table: "dbo.Budget", name: "Creator_Id", newName: "User_Id");
            RenameColumn(table: "dbo.AspNetUsers", name: "Owner_Id", newName: "PersonalWallet_Guid");
            RenameIndex(table: "dbo.Budget", name: "IX_Creator_Id", newName: "IX_User_Id");
            AlterColumn("dbo.BudgetAccessRight", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Budget", "Currency_Guid", c => c.Guid(nullable: false));
            AlterColumn("dbo.WalletAccessRight", "Wallet_Guid", c => c.Guid(nullable: false));
            CreateIndex("dbo.BudgetAccessRight", "User_Id");
            CreateIndex("dbo.Budget", "Currency_Guid");
            CreateIndex("dbo.WalletAccessRight", "Wallet_Guid");
            CreateIndex("dbo.AspNetUsers", "PersonalWallet_Guid");
            DropColumn("dbo.Wallet", "Owner_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Wallet", "Owner_Id", c => c.String(nullable: false, maxLength: 128));
            DropIndex("dbo.AspNetUsers", new[] { "PersonalWallet_Guid" });
            DropIndex("dbo.WalletAccessRight", new[] { "Wallet_Guid" });
            DropIndex("dbo.Budget", new[] { "Currency_Guid" });
            DropIndex("dbo.BudgetAccessRight", new[] { "User_Id" });
            AlterColumn("dbo.WalletAccessRight", "Wallet_Guid", c => c.Guid());
            AlterColumn("dbo.Budget", "Currency_Guid", c => c.Guid());
            AlterColumn("dbo.BudgetAccessRight", "User_Id", c => c.String(nullable: false, maxLength: 128));
            RenameIndex(table: "dbo.Budget", name: "IX_User_Id", newName: "IX_Creator_Id");
            RenameColumn(table: "dbo.AspNetUsers", name: "PersonalWallet_Guid", newName: "Owner_Id");
            RenameColumn(table: "dbo.Budget", name: "User_Id", newName: "Creator_Id");
            CreateIndex("dbo.WalletAccessRight", "Wallet_Guid");
            CreateIndex("dbo.Wallet", "Owner_Id");
            CreateIndex("dbo.Budget", "Currency_Guid");
            CreateIndex("dbo.BudgetAccessRight", "User_Id");
        }
    }
}
