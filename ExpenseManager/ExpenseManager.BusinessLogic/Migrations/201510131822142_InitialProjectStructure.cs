namespace ExpenseManager.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialProjectStructure : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BudgetAccessRight",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Permission = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        BudgetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Budget", t => t.BudgetId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.BudgetId);
            
            CreateTable(
                "dbo.Budget",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Limit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatorId = c.Int(nullable: false),
                        CurrencyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatorId)
                .ForeignKey("dbo.Currency", t => t.CurrencyId)
                .Index(t => t.CreatorId)
                .Index(t => t.CurrencyId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        PersonalWalletId = c.Int(nullable: false),
                        Wallet_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Wallet", t => t.Wallet_Id)
                .ForeignKey("dbo.Wallet", t => t.PersonalWalletId)
                .Index(t => t.PersonalWalletId)
                .Index(t => t.Wallet_Id);
            
            CreateTable(
                "dbo.Wallet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CurrencyId = c.Int(nullable: false),
                        Owner_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currency", t => t.CurrencyId)
                .ForeignKey("dbo.User", t => t.Owner_Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.CurrencyId)
                .Index(t => t.Owner_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Currency",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Symbol = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transaction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceiptImage = c.String(),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        WalletId = c.Int(nullable: false),
                        BudgetId = c.Int(nullable: false),
                        CurrencyId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Budget", t => t.BudgetId)
                .ForeignKey("dbo.Category", t => t.CategoryId)
                .ForeignKey("dbo.Currency", t => t.CurrencyId)
                .ForeignKey("dbo.Wallet", t => t.WalletId)
                .Index(t => t.WalletId)
                .Index(t => t.BudgetId)
                .Index(t => t.CurrencyId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Icon = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Budget", "CurrencyId", "dbo.Currency");
            DropForeignKey("dbo.User", "PersonalWalletId", "dbo.Wallet");
            DropForeignKey("dbo.Budget", "CreatorId", "dbo.User");
            DropForeignKey("dbo.BudgetAccessRight", "UserId", "dbo.User");
            DropForeignKey("dbo.Wallet", "User_Id", "dbo.User");
            DropForeignKey("dbo.User", "Wallet_Id", "dbo.Wallet");
            DropForeignKey("dbo.Transaction", "WalletId", "dbo.Wallet");
            DropForeignKey("dbo.Transaction", "CurrencyId", "dbo.Currency");
            DropForeignKey("dbo.Transaction", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.Transaction", "BudgetId", "dbo.Budget");
            DropForeignKey("dbo.Wallet", "Owner_Id", "dbo.User");
            DropForeignKey("dbo.Wallet", "CurrencyId", "dbo.Currency");
            DropForeignKey("dbo.BudgetAccessRight", "BudgetId", "dbo.Budget");
            DropIndex("dbo.Transaction", new[] { "CategoryId" });
            DropIndex("dbo.Transaction", new[] { "CurrencyId" });
            DropIndex("dbo.Transaction", new[] { "BudgetId" });
            DropIndex("dbo.Transaction", new[] { "WalletId" });
            DropIndex("dbo.Wallet", new[] { "User_Id" });
            DropIndex("dbo.Wallet", new[] { "Owner_Id" });
            DropIndex("dbo.Wallet", new[] { "CurrencyId" });
            DropIndex("dbo.User", new[] { "Wallet_Id" });
            DropIndex("dbo.User", new[] { "PersonalWalletId" });
            DropIndex("dbo.Budget", new[] { "CurrencyId" });
            DropIndex("dbo.Budget", new[] { "CreatorId" });
            DropIndex("dbo.BudgetAccessRight", new[] { "BudgetId" });
            DropIndex("dbo.BudgetAccessRight", new[] { "UserId" });
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
