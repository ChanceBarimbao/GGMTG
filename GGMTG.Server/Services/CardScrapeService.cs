using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System;

namespace GGMTG.Services
{
    // A simple model for a Scryfall "Card" object
    public class ScryfallCard
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ManaCost { get; set; }
        public string TypeLine { get; set; }
        public string OracleText { get; set; }
        // Add or remove fields as you wish
    }

    // The Scryfall "list" object: { object: "list", has_more: bool, next_page: url, data: [...] }
    // Added Next_Page for pagination
    public class ScryfallList<T>
    {
        public string Object { get; set; }
        public bool Has_More { get; set; }
        public string Next_Page { get; set; }
        public List<T> Data { get; set; }
    }

    public class ScryfallSet
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string SearchUri { get; set; }
        public string ReleasedAt { get; set; }
        // ...
    }

    // Keep the class name EXACTLY as "CardScrapeService"
    public class CardScrapeService
    {
        private readonly HttpClient _httpClient;

        public CardScrapeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Optionally add a User-Agent to avoid any 400 issues
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GGMTG/1.0");
        }

        // 1) Get a list of all sets
        public async Task<List<ScryfallSet>> GetAllScryfallSetsAsync()
        {
            // The Scryfall "all sets" endpoint
            string url = "https://api.scryfall.com/sets";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<ScryfallList<ScryfallSet>>(jsonString, options);
            return result?.Data ?? new List<ScryfallSet>();
        }

        // 2) (Optional) Retrieve all cards in a single set’s SearchUri
        //    This method handles pagination automatically.
        public async Task<List<ScryfallCard>> GetAllCardsForSetAsync(string searchUri)
        {
            var allCards = new List<ScryfallCard>();
            var currentUri = searchUri;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            while (!string.IsNullOrEmpty(currentUri))
            {
                var response = await _httpClient.GetAsync(currentUri);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var page = JsonSerializer.Deserialize<ScryfallList<ScryfallCard>>(json, options);

                if (page?.Data != null)
                {
                    allCards.AddRange(page.Data);
                }

                // If there's another page, update currentUri to next_page
                if (page?.Has_More == true && !string.IsNullOrEmpty(page.Next_Page))
                {
                    currentUri = page.Next_Page;
                }
                else
                {
                    // No more pages
                    currentUri = null;
                }
            }
            //return Ok(jsonString);
            return allCards;
        }

        // 3) NEW: Retrieve every set, then gather all cards in each set
        //    Returns a dictionary: key = set code (or ID), value = list of that set’s cards
        public async Task<Dictionary<string, List<ScryfallCard>>> GetAllCardsInAllSetsAsync()
        {
            // A) Get all sets
            var sets = await GetAllScryfallSetsAsync();
            // Prepare the final dictionary
            var results = new Dictionary<string, List<ScryfallCard>>();

            // B) For each set, load all cards
            foreach (var scrySet in sets)
            {
                // The set code might be null (rare). Use set.Id as a fallback
                var setKey = !string.IsNullOrEmpty(scrySet.Code) ? scrySet.Code : scrySet.Id;

                // If the set has a SearchUri, fetch all its cards
                if (!string.IsNullOrEmpty(scrySet.SearchUri))
                {
                    var cards = await GetAllCardsForSetAsync(scrySet.SearchUri);
                    results[setKey] = cards;
                }
                else
                {
                    // If for some reason SearchUri is missing, store empty
                    results[setKey] = new List<ScryfallCard>();
                }
            }

            return results;
        }
    }
}
