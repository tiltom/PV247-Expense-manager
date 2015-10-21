namespace ExpenseManager.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BudgetUpdate : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Budget", new[] { "Creator_Id" });
            DropIndex("dbo.Budget", new[] { "Currency_Guid" });
            AlterColumn("dbo.Budget", "Creator_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Budget", "Currency_Guid", c => c.Guid());
            CreateIndex("dbo.Budget", "Creator_Id");
            CreateIndex("dbo.Budget", "Currency_Guid");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Budget", new[] { "Currency_Guid" });
            DropIndex("dbo.Budget", new[] { "Creator_Id" });
            AlterColumn("dbo.Budget", "Currency_Guid", c => c.Guid(nullable: false));
            AlterColumn("dbo.Budget", "Creator_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Budget", "Currency_Guid");
            CreateIndex("dbo.Budget", "Creator_Id");
        }
    }
}
