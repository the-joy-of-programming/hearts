using System.ComponentModel.DataAnnotations;

namespace HeartsApp
{
    public class AppStatistics
    {
        public int Id { get; set; }
        [ConcurrencyCheck]
        public int ShuffleCount { get; set; }
    }
}