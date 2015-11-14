namespace ExpenseManager.Database.Seeding.Context.SeedingContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RepeatableTransactionFixes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RepeatableTransactions", "NextRepeat", c => c.Int(nullable: false));
            AddColumn("dbo.RepeatableTransactions", "FrequencyType", c => c.Int(nullable: false));
            DropColumn("dbo.RepeatableTransactions", "Frequency");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RepeatableTransactions", "Frequency", c => c.Time(nullable: false, precision: 7));
            DropColumn("dbo.RepeatableTransactions", "FrequencyType");
            DropColumn("dbo.RepeatableTransactions", "NextRepeat");
        }
    }
}
