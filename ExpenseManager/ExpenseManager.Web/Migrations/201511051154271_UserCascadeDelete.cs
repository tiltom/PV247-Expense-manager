namespace ExpenseManager.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserCascadeDelete : DbMigration
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
                        UserProfile_Guid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.Budget", t => t.Budget_Guid, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.UserProfile_Guid)
                .Index(t => t.Budget_Guid)
                .Index(t => t.UserProfile_Guid);
            
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
                .ForeignKey("dbo.UserProfile", t => t.Creator_Guid, cascadeDelete: true)
                .ForeignKey("dbo.Currency", t => t.Currency_Guid)
                .Index(t => t.Creator_Guid)
                .Index(t => t.Currency_Guid);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        Guid = c.Guid(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        UserIdentity_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.AspNetUsers", t => t.UserIdentity_Id, cascadeDelete: true)
                .Index(t => t.UserIdentity_Id);
            
            CreateTable(
                "dbo.Wallet",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Name = c.String(),
                        Currency_Guid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.Currency", t => t.Currency_Guid)
                .ForeignKey("dbo.UserProfile", t => t.Guid, cascadeDelete: true)
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
                .ForeignKey("dbo.Wallet", t => t.Wallet_Guid, cascadeDelete: true)
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
                        UserProfile_Guid = c.Guid(nullable: false),
                        Wallet_Guid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.UserProfile", t => t.UserProfile_Guid)
                .ForeignKey("dbo.Wallet", t => t.Wallet_Guid, cascadeDelete: true)
                .Index(t => t.UserProfile_Guid)
                .Index(t => t.Wallet_Guid);
            
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
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreationDate = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserProfile", "UserIdentity_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.RepeatableTransaction", "FirstTransaction_Guid", "dbo.Transaction");
            DropForeignKey("dbo.BudgetAccessRight", "UserProfile_Guid", "dbo.UserProfile");
            DropForeignKey("dbo.Budget", "Currency_Guid", "dbo.Currency");
            DropForeignKey("dbo.Wallet", "Guid", "dbo.UserProfile");
            DropForeignKey("dbo.WalletAccessRight", "Wallet_Guid", "dbo.Wallet");
            DropForeignKey("dbo.WalletAccessRight", "UserProfile_Guid", "dbo.UserProfile");
            DropForeignKey("dbo.Transaction", "Wallet_Guid", "dbo.Wallet");
            DropForeignKey("dbo.Transaction", "Currency_Guid", "dbo.Currency");
            DropForeignKey("dbo.Transaction", "Category_Guid", "dbo.Category");
            DropForeignKey("dbo.Transaction", "Budget_Guid", "dbo.Budget");
            DropForeignKey("dbo.Wallet", "Currency_Guid", "dbo.Currency");
            DropForeignKey("dbo.Budget", "Creator_Guid", "dbo.UserProfile");
            DropForeignKey("dbo.BudgetAccessRight", "Budget_Guid", "dbo.Budget");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.RepeatableTransaction", new[] { "FirstTransaction_Guid" });
            DropIndex("dbo.WalletAccessRight", new[] { "Wallet_Guid" });
            DropIndex("dbo.WalletAccessRight", new[] { "UserProfile_Guid" });
            DropIndex("dbo.Transaction", new[] { "Wallet_Guid" });
            DropIndex("dbo.Transaction", new[] { "Currency_Guid" });
            DropIndex("dbo.Transaction", new[] { "Category_Guid" });
            DropIndex("dbo.Transaction", new[] { "Budget_Guid" });
            DropIndex("dbo.Wallet", new[] { "Currency_Guid" });
            DropIndex("dbo.Wallet", new[] { "Guid" });
            DropIndex("dbo.UserProfile", new[] { "UserIdentity_Id" });
            DropIndex("dbo.Budget", new[] { "Currency_Guid" });
            DropIndex("dbo.Budget", new[] { "Creator_Guid" });
            DropIndex("dbo.BudgetAccessRight", new[] { "UserProfile_Guid" });
            DropIndex("dbo.BudgetAccessRight", new[] { "Budget_Guid" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RepeatableTransaction");
            DropTable("dbo.WalletAccessRight");
            DropTable("dbo.Category");
            DropTable("dbo.Transaction");
            DropTable("dbo.Currency");
            DropTable("dbo.Wallet");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Budget");
            DropTable("dbo.BudgetAccessRight");
        }
    }
}
