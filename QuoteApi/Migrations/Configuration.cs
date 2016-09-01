using QuoteApi.Models;

namespace QuoteApi.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<QuoteApi.Models.QuoteApiContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(QuoteApi.Models.QuoteApiContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            context.SingleQuotes.AddOrUpdate(p => p.Quote, new SingleQuote
            {
                Quote = "Test quote bằng tiếng Việt",
            },
            new SingleQuote
            {
                Quote = "Test quote bằng tiếng Việt 2",
            });
        }
    }
}
