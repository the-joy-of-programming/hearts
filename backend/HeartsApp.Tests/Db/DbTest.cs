using Microsoft.EntityFrameworkCore;

namespace HeartsApp
{
    public class DbTest : BaseTest
    {
        public DbTest()
        {
            using (var context = CreateHeartsContext())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE \"AppStatistics\" RESTART IDENTITY");
            }
        }

        protected HeartsContext CreateHeartsContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<HeartsContext>();
            ConfigureDatabase(optionsBuilder);
            return new HeartsContext(optionsBuilder.Options);
        }

        private void ConfigureDatabase(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
              .UseNpgsql("Server=127.0.0.1;Port=5432;Database=hearts;User Id=postgres;Password=Xsw23edc;")
              .UseLoggerFactory(loggerFactory);
        }
    }
}