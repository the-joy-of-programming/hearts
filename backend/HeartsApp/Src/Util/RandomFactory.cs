using System;

namespace HeartsApp
{
    public class RandomFactory
    {

        private Random masterRandom;

        public RandomFactory(int? seed = null)
        {
            if (seed == null)
            {
                masterRandom = new Random();
            }
            else
            {
                masterRandom = new Random((int)seed);
            }
        }

        public Random createRandom()
        {
            lock(masterRandom)
            {
                return new Random(masterRandom.Next());
            }
        }

    }
}