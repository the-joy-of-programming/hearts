using System;

namespace Hearts
{
    public class BaseTest
    {
        protected Random UnitTestRandom { get; } = new Random(42);
    }
}