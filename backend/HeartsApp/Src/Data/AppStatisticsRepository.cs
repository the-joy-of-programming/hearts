using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeartsApp
{
    public class AppStatisticsRepository
    {

        private readonly HeartsContext heartsContext;
        private readonly ILogger<AppStatisticsRepository> logger;

        public AppStatisticsRepository(HeartsContext heartsContext, ILogger<AppStatisticsRepository> logger)
        {
            this.heartsContext = heartsContext;
            this.logger = logger;
        }

        public Task<AppStatistics> GetSingletonAsync()
        {
            return this.heartsContext.AppStatistics.FirstAsync();
        }

        public async Task<int> IncrementShuffleCountAsync()
        {
            var appStatistics = await GetSingletonAsync();
            appStatistics.ShuffleCount++;
            await OptimisticRetry.SaveChangesWithRetryAsync(
                heartsContext,
                async (entries) =>
                {
                    var dbValues = await entries[0].GetDatabaseValuesAsync();
                    var dbValue = (int) dbValues["ShuffleCount"];
                    var result = dbValue + 1;
                    entries[0].CurrentValues["ShuffleCount"] = result;
                    entries[0].OriginalValues["ShuffleCount"] = dbValue;
                },
                logger
            );
            return appStatistics.ShuffleCount;
        }
        
    }
}