using System.Threading.Tasks;

namespace HeartsApp.Seeders
{
    public class AppStatisticsSeeder : ISeeder
    {
        public string Name { get; } = "AppStatisticsSeeder";

        public async Task SeedDb(HeartsContext context)
        {
            var initialStatistics = new AppStatistics
            {
                ShuffleCount = 0
            };
            await context.AppStatistics.AddAsync(initialStatistics);
            await context.SaveChangesAsync();
        }
    }
}