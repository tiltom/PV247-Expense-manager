namespace ExpenseManager.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedNullableColumns : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.User", new[] { "PersonalWalletId" });
            DropIndex("dbo.Transaction", new[] { "BudgetId" });
            AlterColumn("dbo.User", "PersonalWalletId", c => c.Int());
            AlterColumn("dbo.Transaction", "BudgetId", c => c.Int());
            CreateIndex("dbo.User", "PersonalWalletId");
            CreateIndex("dbo.Transaction", "BudgetId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Transaction", new[] { "BudgetId" });
            DropIndex("dbo.User", new[] { "PersonalWalletId" });
            AlterColumn("dbo.Transaction", "BudgetId", c => c.Int(nullable: false));
            AlterColumn("dbo.User", "PersonalWalletId", c => c.Int(nullable: false));
            CreateIndex("dbo.Transaction", "BudgetId");
            CreateIndex("dbo.User", "PersonalWalletId");
        }
    }
}
