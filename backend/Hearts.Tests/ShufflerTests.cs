using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Hearts
{
    public class ShufflerTests : BaseTest
    {

        private Shuffler shuffler;

        [SetUp]
        public void Init()
        {
            shuffler = new Shuffler(UnitTestRandom);
        }

        [Test]
        public void ShouldShuffleCardsUniformly()
        {
            var cards = new List<PlayingCard>() 
            { 
                new PlayingCard { Suit = Suit.Hearts, Rank = 1 },
                new PlayingCard { Suit = Suit.Hearts, Rank = 2 },
                new PlayingCard { Suit = Suit.Hearts, Rank = 3 }
            };
            var combinationNumberToCounts = new Dictionary<int, int>();
            for (var combinationNumber = 1; combinationNumber <= 6; combinationNumber++)
            {
                combinationNumberToCounts[combinationNumber] = 0;
            }
            for (var i = 0; i < 1000; i++)
            {
                shuffler.Shuffle(cards);
                var combinationNumber = GetCombinationNumber(cards);
                combinationNumberToCounts[combinationNumber]++;
            }
            var smallestCount = combinationNumberToCounts.Values.Min();
            var largestCount = combinationNumberToCounts.Values.Max();
            var difference = largestCount - smallestCount;
            Assert.LessOrEqual(difference, 50, "The shuffler does not appear to be shuffling uniformly");
        }

        private int GetCombinationNumber(List<PlayingCard> cards)
        {
            if (cards[0].Rank == 1 && cards[1].Rank == 2) return 1;
            else if (cards[0].Rank == 1) return 2;
            else if (cards[0].Rank == 2 && cards[1].Rank == 1) return 3;
            else if (cards[0].Rank == 2) return 4;
            else if (cards[0].Rank == 3 && cards[1].Rank == 1) return 5;
            else return 6;
        }

    }
}