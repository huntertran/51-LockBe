namespace QuoteApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SingleQuotes",
                c => new
                    {
                        SingleQuoteId = c.Int(nullable: false, identity: true),
                        Quote = c.String(),
                        Self = c.String(),
                    })
                .PrimaryKey(t => t.SingleQuoteId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SingleQuotes");
        }
    }
}
