using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HeartsApp
{
    public class HeartsContextFactory : IDesignTimeDbContextFactory<HeartsContext>
    {
        public HeartsContext CreateDbContext(string[] args)
        {
            var configuration = LoadConfig();
            var connectionString = configuration["Database:ConnectionString"];
            if (connectionString == null)
            {
                throw new Exception("The property Database:ConnectionString must be specified");
            }
            var optionsBuilder = new DbContextOptionsBuilder<HeartsContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new HeartsContext(optionsBuilder.Options);
        }

        private IConfiguration LoadConfig()
        {
            return new ConfigurationBuilder()
              .AddUserSecrets<HeartsContextFactory>()
              .AddEnvironmentVariables()
              .Build();
        }
    }
}