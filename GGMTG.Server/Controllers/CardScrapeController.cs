using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using GGMTG.Services;

namespace GGMTG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardScrapeController : ControllerBase
    {
        private readonly CardScrapeService _scrapeService;

        public CardScrapeController(CardScrapeService scrapeService)
        {
            _scrapeService = scrapeService;
        }

        // GET: api/CardScrape/sets
        // Returns basic info for all Scryfall sets
        [HttpGet("sets")]
        public async Task<ActionResult<List<ScryfallSet>>> GetScryfallSets()
        {
            var sets = await _scrapeService.GetAllScryfallSetsAsync();
            return Ok(sets);
        }

        //GET: api/CardScrape/cards? uri = ...
        // Returns all cards(in JSON) for a single set’s searchUri
        [HttpGet("cards")]
        public async Task<ActionResult<string>> GetCards([FromQuery] string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return BadRequest("No URI provided.");

            var cardData = await _scrapeService.GetAllCardsForSetAsync(uri);
            return Ok(cardData);
        }

        // NEW ENDPOINT: GET every set, then retrieve ALL cards from each set
        // WARNING: Potentially large result, might want to implement caching / partial loading.
        // GET: api/CardScrape/all-cards
        [HttpGet("all-cards")]
        public async Task<ActionResult<Dictionary<string, List<ScryfallCard>>>> GetAllCardsInAllSets()
        {
            // This calls the new service method that loops all sets and fetches all cards
            var allCards = await _scrapeService.GetAllCardsInAllSetsAsync();
            return Ok(allCards);
        }
    }
}
