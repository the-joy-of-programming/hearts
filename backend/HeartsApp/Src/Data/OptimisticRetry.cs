using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace HeartsApp
{
    public class OptimisticRetry
    {
        private const int TimeoutMs = 30 * 1000;
        public static async Task SaveChangesWithRetryAsync(DbContext dbContext, Func<IReadOnlyList<EntityEntry>, Task> repair, ILogger logger)
        {
            var startTime = DateTime.Now;
            var numAttempts = 0;
            while (true)
            {
                try
                {
                    numAttempts++;
                    await dbContext.SaveChangesAsync();
                    if (numAttempts > 1)
                    {
                        logger.LogWarning($"Optimistic locking exceptions occurred. Successfully persisted data after {numAttempts} attempts");
                    }
                    return;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (DateTime.Now.Subtract(startTime).TotalMilliseconds > TimeoutMs)
                    {
                        throw new Exception($"After {TimeoutMs}ms of retrying we still could not complete the operation due to concurrency conflicts.");
                    }
                    await repair(ex.Entries);
                }
            }
        }
    }
}