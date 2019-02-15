using System.Collections.Generic;
using System.Threading.Tasks;
using Hearts;

namespace HeartsApp
{
    public class ShuffledDeckWithShuffleCount
    {
        public int ShuffleCount { get; set; }
        public List<PlayingCard> Cards { get; set; }
    }

    public class GetShuffledDeckUseCase
    {
        private Shuffler shuffler;
        private AppStatisticsRepository appStatisticsRepository;

        public GetShuffledDeckUseCase(Shuffler shuffler, AppStatisticsRepository appStatisticsRepository)
        {
            this.shuffler = shuffler;
            this.appStatisticsRepository = appStatisticsRepository;
        }

        public async Task<ShuffledDeckWithShuffleCount> Go()
        {
            var shuffleCount = await appStatisticsRepository.IncrementShuffleCountAsync();
            var cards = PlayingCard.NewDeck();
            shuffler.Shuffle(cards);
            return new ShuffledDeckWithShuffleCount
            {
                ShuffleCount = shuffleCount,
                Cards = cards
            };
        }
    }
}