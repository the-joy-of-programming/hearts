using System;
using System.Collections.Generic;

namespace Hearts
{
    public class Shuffler
    {

        private Random random;

        public Shuffler(Random random)
        {
            this.random = random;
        }

        public void Shuffle(List<PlayingCard> cards)
        {
            var n = cards.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                var value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }

    }
}