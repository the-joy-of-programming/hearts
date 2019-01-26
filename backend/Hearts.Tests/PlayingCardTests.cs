using System;
using System.Linq;
using NUnit.Framework;

namespace Hearts
{

    public class PlayingCardTests
    {

        [Test]
        public void ShouldGenerateANewDeck()
        {
            var deck = PlayingCard.NewDeck();
            Assert.AreEqual(52, deck.Count);
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                Assert.AreEqual(13, deck.Where(card => card.Suit == suit).Count(), $"Expected there to be 13 instances of the suit {suit.ToString()} in a new deck");
            }
            for (var rank = PlayingCard.MinRank; rank <= PlayingCard.MaxRank; rank++)
            {
                Assert.AreEqual(4, deck.Where(card => card.Rank == rank).Count(), $"Expected there to be 4 cards with the rank {rank} in a new deck");
            }
        }

    }

}