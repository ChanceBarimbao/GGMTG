//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using AngleSharp;
//using AngleSharp.Dom;

//namespace GGMTG.Services
//{
//    // A simple DTO (Data Transfer Object) or record to hold card info 
//    // without creating a separate folder for "Models".
//    public record CardData(string Name, string SetName, string Rarity, decimal Price);

//    public class CardScrapeService
//    {
//        private readonly HttpClient _httpClient;

//        public CardScrapeService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        public async Task<List<CardData>> ScrapeCardsAsync()
//        {
//            // 1. Prepare a list to hold results
//            var cards = new List<CardData>();

//            // 2. TCGplayer URL for demonstration (e.g., searching Magic cards, page 1).
//            //    Inspect TCGplayer’s HTML to ensure the selectors below match the real structure.
//            string url = "https://www.tcgplayer.com/search/magic/product?productLineName=magic&view=grid";

//            // 3. Get the raw HTML
//            var html = await _httpClient.GetStringAsync(url);
//            Console.WriteLine(html);
//            // 4. Parse HTML via AngleSharp
//            var config = Configuration.Default;
//            using var context = BrowsingContext.New(config);
//            using var doc = await context.OpenAsync(req => req.Content(html));

//            // 5. Query the DOM for card elements
//            //    The selector ".search-result__content" is hypothetical; TCGplayer might differ.
//            var cardElements = doc.QuerySelectorAll(".search-result__content");
//            foreach (var element in cardElements)
//            {
//                // A. Extract card name
//                var name = element.QuerySelector(".search-result__title")
//                            ?.TextContent?.Trim() ?? "Unknown";

//                // B. Extract set name
//                var setName = element.QuerySelector(".search-result__subtitle")
//                               ?.TextContent?.Trim() ?? "Unknown";

//                // C. Extract rarity (example)
//                var rarity = element.QuerySelector(".rarity")
//                              ?.TextContent?.Trim() ?? "Unknown";

//                // D. Extract price
//                var priceStr = element.QuerySelector(".search-result__market-price--value")
//                               ?.TextContent?.Trim() ?? "0";

//                if (!decimal.TryParse(priceStr.Replace("$", ""), out decimal price))
//                {
//                    price = 0;
//                }

//                // E. Add to list
//                cards.Add(new CardData(name, setName, rarity, price));
//            }

//            return cards;
//        }
//    }
//}
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Text.Json; // or Newtonsoft.Json
//using System;

//namespace GGMTG.Services
//{
//    // SAMPLE model for a "Set" from Scryfall's /sets endpoint.
//    // Add/remove properties as needed (see Scryfall docs).
//    public class ScryfallSet
//    {
//        public string Id { get; set; }
//        public string Code { get; set; }
//        public string Name { get; set; }
//        public string SearchUri { get; set; }
//        public string ReleasedAt { get; set; }
//        // ...
//    }

//    // The Scryfall "list" object usually looks like: { "object": "list", "has_more": false, "data": [ ... ] }
//    public class ScryfallList<T>
//    {
//        public string Object { get; set; }
//        public bool Has_More { get; set; }
//        public List<T> Data { get; set; }
//    }

//    // Keep the service name EXACTLY as "CardScrapeService" 
//    public class CardScrapeService
//    {
//        private readonly HttpClient _httpClient;

//        public CardScrapeService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        // 1) Get a list of all sets via Scryfall's official JSON endpoint
//        //    https://api.scryfall.com/sets
//        public async Task<List<ScryfallSet>> GetAllScryfallSetsAsync()
//        {
//            // The Scryfall "all sets" endpoint:
//            string url = "https://api.scryfall.com/sets";

//            // Make the GET request
//            var response = await _httpClient.GetAsync(url);
//            response.EnsureSuccessStatusCode();

//            // Read response as string
//            var jsonString = await response.Content.ReadAsStringAsync();

//            // Deserialize into ScryfallList<ScryfallSet>
//            var options = new JsonSerializerOptions
//            {
//                PropertyNameCaseInsensitive = true
//            };
//            var result = JsonSerializer.Deserialize<ScryfallList<ScryfallSet>>(jsonString, options);

//            // Return the list of sets
//            return result?.Data ?? new List<ScryfallSet>();
//        }

//        // 2) (Optional) For each set’s "search_uri", you can retrieve its cards
//        //    See https://scryfall.com/docs/api/cards for more details
//        public async Task<string> GetCardsForSetAsync(string searchUri)
//        {
//            // Example: searchUri might be "https://api.scryfall.com/cards/search?order=set&q=e%3Aneo&unique=prints"

//            var response = await _httpClient.GetAsync(searchUri);
//            response.EnsureSuccessStatusCode();

//            // For demonstration, we'll just return the raw JSON string
//            // but you can create a "ScryfallCard" model and deserialize as well.
//            string cardJson = await response.Content.ReadAsStringAsync();
//            return cardJson;
//        }
//    }
//}
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Text.Json; // or Newtonsoft.Json
//using System;

//namespace GGMTG.Services
//{
//    // SAMPLE model for a "Set" from Scryfall's /sets endpoint.
//    // Add/remove properties as needed (see Scryfall docs).
//    public class ScryfallSet
//    {
//        public string Id { get; set; }
//        public string Code { get; set; }
//        public string Name { get; set; }
//        public string SearchUri { get; set; }
//        public string ReleasedAt { get; set; }
//        // ...
//    }

//    // The Scryfall "list" object usually looks like: { "object": "list", "has_more": false, "data": [ ... ] }
//    public class ScryfallList<T>
//    {
//        public string Object { get; set; }
//        public bool Has_More { get; set; }
//        public List<T> Data { get; set; }
//    }

//    // Keep the service name EXACTLY as "CardScrapeService" 
//    public class CardScrapeService
//    {
//        private readonly HttpClient _httpClient;

//        public CardScrapeService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        // 1) Get a list of all sets via Scryfall's official JSON endpoint
//        //    https://api.scryfall.com/sets
//        public async Task<List<ScryfallSet>> GetAllScryfallSetsAsync()
//        {
//            // The Scryfall "all sets" endpoint:
//            string url = "https://api.scryfall.com/sets";

//            // Make the GET request
//            var response = await _httpClient.GetAsync(url);
//            response.EnsureSuccessStatusCode();

//            // Read response as string
//            var jsonString = await response.Content.ReadAsStringAsync();

//            // Deserialize into ScryfallList<ScryfallSet>
//            var options = new JsonSerializerOptions
//            {
//                PropertyNameCaseInsensitive = true
//            };
//            var result = JsonSerializer.Deserialize<ScryfallList<ScryfallSet>>(jsonString, options);

//            // Return the list of sets
//            return result?.Data ?? new List<ScryfallSet>();
//        }

//        // 2) (Optional) For each set’s "search_uri", you can retrieve its cards
//        //    See https://scryfall.com/docs/api/cards for more details
//        public async Task<string> GetCardsForSetAsync(string searchUri)
//        {
//            // Example: searchUri might be "https://api.scryfall.com/cards/search?order=set&q=e%3Aneo&unique=prints"

//            var response = await _httpClient.GetAsync(searchUri);
//            response.EnsureSuccessStatusCode();

//            // For demonstration, we'll just return the raw JSON string
//            // but you can create a "ScryfallCard" model and deserialize as well.
//            string cardJson = await response.Content.ReadAsStringAsync();
//            return cardJson;
//        }
//    }
//}
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
