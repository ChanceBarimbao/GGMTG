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
        [HttpGet("sets")]
        public async Task<ActionResult<List<ScryfallSet>>> GetScryfallSets()
        {
            var sets = await _scrapeService.GetAllScryfallSetsAsync();
            return Ok(sets);
        }

        // GET: api/CardScrape/cards?uri=...
        [HttpGet("cards")]
        public async Task<ActionResult<string>> GetCards([FromQuery] string uri)
        {
            // Return raw JSON (or parse it further if you like)
            if (string.IsNullOrEmpty(uri))
                return BadRequest("No URI provided.");

            var cardData = await _scrapeService.GetCardsForSetAsync(uri);
            return Ok(cardData);
        }
    }
}
