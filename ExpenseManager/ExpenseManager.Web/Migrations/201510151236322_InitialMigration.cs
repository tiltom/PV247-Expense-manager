namespace ExpenseManager.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BudgetAccessRight",
                c => new
                    {
                        Guid = c.Guid(nullable: false, identity: true),
                        Permission = c.Int(nullable: false),
                        Budget_Guid = c.Guid(nullable: false),
                        User_Guid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.Budget", t => t.Budget_Guid)
                .ForeignKey("dbo.User", t => t.User_Guid)
                .Index(t => t.Budget_Guid)
                .Index(t => t.User_Guid);
            
            CreateTable(
                "dbo.Budget",
                c => new
                    {
                        Guid = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Limit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Creator_Guid = c.Guid(nullable: false),
                        Currency_Guid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.User", t => t.Creator_Guid)
                .ForeignKey("dbo.Currency", t => t.Currency_Guid)
                .Index(t => t.Creator_Guid)
                .Index(t => t.Currency_Guid);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Guid = c.Guid(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "dbo.Wallet",
                c => new
                    {
                        Guid = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Currency_Guid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.Currency", t => t.Currency_Guid)
                .ForeignKey("dbo.User", t => t.Guid)
                .Index(t => t.Guid)
                .Index(t => t.Currency_Guid);
            
            CreateTable(
                "dbo.Currency",
                c => new
                    {
                        Guid = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Symbol = c.String(),
                    })
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "dbo.Transaction",
                c => new
                    {
                        Guid = c.Guid(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        Budget_Guid = c.Guid(),
                        Category_Guid = c.Guid(nullable: false),
                        Currency_Guid = c.Guid(nullable: false),
                        Wallet_Guid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.Budget", t => t.Budget_Guid)
                .ForeignKey("dbo.Category", t => t.Category_Guid)
                .ForeignKey("dbo.Currency", t => t.Currency_Guid)
                .ForeignKey("dbo.Wallet", t => t.Wallet_Guid)
                .Index(t => t.Budget_Guid)
                .Index(t => t.Category_Guid)
                .Index(t => t.Currency_Guid)
                .Index(t => t.Wallet_Guid);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Guid = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IconPath = c.String(),
                    })
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "dbo.WalletAccessRight",
                c => new
                    {
                        Guid = c.Guid(nullable: false, identity: true),
                        Permission = c.Int(nullable: false),
                        Budget_Guid = c.Guid(),
                        User_Guid = c.Guid(),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.Wallet", t => t.Budget_Guid)
                .ForeignKey("dbo.User", t => t.User_Guid)
                .Index(t => t.Budget_Guid)
                .Index(t => t.User_Guid);
            
            CreateTable(
                "dbo.RepeatableTransaction",
                c => new
                    {
                        Guid = c.Guid(nullable: false, identity: true),
                        Frequency = c.Time(nullable: false, precision: 7),
                        LastOccurence = c.DateTime(nullable: false),
                        FirstTransaction_Guid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.Transaction", t => t.FirstTransaction_Guid)
                .Index(t => t.FirstTransaction_Guid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RepeatableTransaction", "FirstTransaction_Guid", "dbo.Transaction");
            DropForeignKey("dbo.BudgetAccessRight", "User_Guid", "dbo.User");
            DropForeignKey("dbo.BudgetAccessRight", "Budget_Guid", "dbo.Budget");
            DropForeignKey("dbo.Budget", "Currency_Guid", "dbo.Currency");
            DropForeignKey("dbo.Budget", "Creator_Guid", "dbo.User");
            DropForeignKey("dbo.WalletAccessRight", "User_Guid", "dbo.User");
            DropForeignKey("dbo.WalletAccessRight", "Budget_Guid", "dbo.Wallet");
            DropForeignKey("dbo.Transaction", "Wallet_Guid", "dbo.Wallet");
            DropForeignKey("dbo.Transaction", "Currency_Guid", "dbo.Currency");
            DropForeignKey("dbo.Transaction", "Category_Guid", "dbo.Category");
            DropForeignKey("dbo.Transaction", "Budget_Guid", "dbo.Budget");
            DropForeignKey("dbo.Wallet", "Guid", "dbo.User");
            DropForeignKey("dbo.Wallet", "Currency_Guid", "dbo.Currency");
            DropIndex("dbo.RepeatableTransaction", new[] { "FirstTransaction_Guid" });
            DropIndex("dbo.WalletAccessRight", new[] { "User_Guid" });
            DropIndex("dbo.WalletAccessRight", new[] { "Budget_Guid" });
            DropIndex("dbo.Transaction", new[] { "Wallet_Guid" });
            DropIndex("dbo.Transaction", new[] { "Currency_Guid" });
            DropIndex("dbo.Transaction", new[] { "Category_Guid" });
            DropIndex("dbo.Transaction", new[] { "Budget_Guid" });
            DropIndex("dbo.Wallet", new[] { "Currency_Guid" });
            DropIndex("dbo.Wallet", new[] { "Guid" });
            DropIndex("dbo.Budget", new[] { "Currency_Guid" });
            DropIndex("dbo.Budget", new[] { "Creator_Guid" });
            DropIndex("dbo.BudgetAccessRight", new[] { "User_Guid" });
            DropIndex("dbo.BudgetAccessRight", new[] { "Budget_Guid" });
            DropTable("dbo.RepeatableTransaction");
            DropTable("dbo.WalletAccessRight");
            DropTable("dbo.Category");
            DropTable("dbo.Transaction");
            DropTable("dbo.Currency");
            DropTable("dbo.Wallet");
            DropTable("dbo.User");
            DropTable("dbo.Budget");
            DropTable("dbo.BudgetAccessRight");
        }
    }
}
