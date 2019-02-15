using System.Threading.Tasks;
using NUnit.Framework;

namespace HeartsApp
{
    public class GetShuffledDeckTest : ApiTest
    {

        [Test]
        public async Task ShouldCreateShuffledDeck()
        {
            var response = await GetAsync<ShuffledDeckWithShuffleCount>("/api");
            Assert.AreEqual(52, response.Cards.Count);
            Assert.AreEqual(1, response.ShuffleCount);
            response = await GetAsync<ShuffledDeckWithShuffleCount>("/api");
            Assert.AreEqual(2, response.ShuffleCount);
        }
    }
}