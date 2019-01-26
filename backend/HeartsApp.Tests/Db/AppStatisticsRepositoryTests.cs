using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace HeartsApp
{
    public class AppStatisticsRepositoryTests : DbTest
    {

        public AppStatisticsRepositoryTests()
        {
            using (var context = CreateHeartsContext())
            {
                context.AppStatistics.Add(new AppStatistics { ShuffleCount = 0 });
                context.SaveChanges();
            }
        }

        [Test]
        public async Task ShouldIncrementShuffleCountAcrossSeveralThreads()
        {
            var tasks = new Task[10];
            for(var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = IncrementTask();
            }
            await Task.WhenAll(tasks);

            using (var context = CreateHeartsContext())
            {
                var repo = new AppStatisticsRepository(context, loggerFactory.CreateLogger<AppStatisticsRepository>());
                var stats = await repo.GetSingletonAsync();
                Assert.AreEqual(10, stats.ShuffleCount);
            }
        }

        private async Task IncrementTask()
        {
            using (var context = CreateHeartsContext())
            {
                await new AppStatisticsRepository(context, loggerFactory.CreateLogger<AppStatisticsRepository>()).IncrementShuffleCountAsync();
            }
        }

    }
}