using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HeartsApp
{
    [Route("/api")]
    public class MainController : Controller
    {
        private GetShuffledDeckUseCase getShuffledDeckUseCase;

        public MainController(GetShuffledDeckUseCase getShuffledDeckUseCase)
        {
            this.getShuffledDeckUseCase = getShuffledDeckUseCase;
        }

        [HttpGet]
        public async Task<ShuffledDeckWithShuffleCount> Get()
        {
            return await this.getShuffledDeckUseCase.Go();
        }
    }
}