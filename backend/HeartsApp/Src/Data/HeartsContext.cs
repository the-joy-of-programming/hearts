using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace HeartsApp
{
    public class HeartsContext : DbContext
    {
        public HeartsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppStatistics> AppStatistics { get; set; }
        public DbSet<SeederRecord> SeederRecord { get; set; }
    }
}