using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeartsApp.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HeartsApp
{
    public class DbInitializationService
    {

        private List<ISeeder> seeders = new List<ISeeder>()
        {
            new AppStatisticsSeeder()
        };

        public async Task Initialize(IServiceProvider serviceProvider)
        {
            await MigrateDb(serviceProvider);
            await SeedDb(serviceProvider);
        }

        private async Task MigrateDb(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<HeartsContext>();
                if (context.Database.IsInMemory())
                {
                    await context.Database.EnsureCreatedAsync();
                }
                else
                {
                    await context.Database.MigrateAsync();
                }
            }
        }

        private async Task SeedDb(IServiceProvider serviceProvider)
        {
            Dictionary<string, SeederRecord> existingSeeders;
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<HeartsContext>();
                existingSeeders = await context.SeederRecord.ToDictionaryAsync(record => record.Id, record => record);
            }
            foreach(var seeder in this.seeders)
            {
                if (!existingSeeders.ContainsKey(seeder.Name))
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetService<HeartsContext>();
                        await context.SeederRecord.AddAsync(new SeederRecord { Id = seeder.Name });
                        await context.SaveChangesAsync();
                        await seeder.SeedDb(context);
                    }
                }
            }
        }

    }

}