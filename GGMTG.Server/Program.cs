using GGMTG.Services;
using Microsoft.EntityFrameworkCore;
using GGMTG.Server.Models;
using Hangfire.SQLite; // Add this namespace for SQLite storage
using GGMTG.Server;
using GGMTG.Server.Repositories;
using Microsoft.OpenApi.Models;
using System.Reflection;
using GGMTG.Server.Controllers;
using GGMTG.Server.Services;
using System.Net;
using Hangfire;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine(builder.Configuration.GetConnectionString("HangfireConnection"));

builder.Services.AddHangfireServer();

// Ensure that the configuration reads appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Register DbContext service (Context)
builder.Services.AddDbContext<Context>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("HangfireConnection")));

// Add Hangfire services and configure Hangfire to use SQLite
builder.Services.AddHangfire(x =>
    x.UseSQLiteStorage(builder.Configuration.GetConnectionString("HangfireConnection")));



// Register services for account, security, and other repositories
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<ISecurityService, SecurityService>();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Code Blueprints API",
        Description = "API Documentation",
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Register Hangfire's JobActivator for Dependency Injection
//builder.Services.AddSingleton<JobActivator>(serviceProvider => new HangfireJobActivator(serviceProvider));

var app = builder.Build();

// Set up the Hangfire dashboard to monitor jobs
app.UseHangfireDashboard();

// Ensure that the database is created
using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetRequiredService<Context>();
dbContext.Database.EnsureCreated();

// Configure middleware
app.UseDefaultFiles();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();

app.UseRouting();
app.UseAuthorization();

// Set up Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Blueprints API V1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();
app.MapFallbackToFile("/index.html");

// Schedule the daily job to fetch and save cards to the database
RecurringJob.AddOrUpdate<RecurringJobsService>(service => service.FetchAndSaveCardsAsync(), Cron.Daily);

app.Run();

// Implement the job to fetch cards from Scryfall and save them to the database
public class RecurringJobsService
{
    private readonly Context _dbContext;

    public RecurringJobsService(Context dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task FetchAndSaveCardsAsync()
    {
        var httpClient = new HttpClient();
        string nextPage = "https://api.scryfall.com/cards";
        var allCards = new List<Card>();

        try
        {
            while (nextPage != null)
            {
                var response = await httpClient.GetStringAsync(nextPage);
                var cards = ParseScryfallResponse(response);
                allCards.AddRange(cards);

                // Get the URL for the next page of cards (pagination)
                nextPage = GetNextPageUrl(response);
            }

            Console.WriteLine($"Fetched {allCards.Count} cards to save");

            // Save all the cards into the database
            _dbContext.Cards.AddRange(allCards);
            await _dbContext.SaveChangesAsync();

            Console.WriteLine($"Successfully saved {allCards.Count} cards to the database.");
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., logging)
            Console.Error.WriteLine($"Error fetching and saving cards: {ex.Message}");
        }
    }



    // Parse Scryfall response to map data to your Card model
    private static IEnumerable<Card> ParseScryfallResponse(string response)
    {
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response);
        var cardList = new List<Card>();

        foreach (var cardData in jsonResponse.GetProperty("data").EnumerateArray())
        {
            var card = new Card
            {
                Id = Guid.NewGuid(),
                Name = cardData.GetProperty("name").GetString(),
                SetCode = cardData.GetProperty("set").GetString(),
                SetName = cardData.GetProperty("set_name").GetString(),
                ManaCost = cardData.GetProperty("mana_cost").GetString(),
                TypeLine = cardData.GetProperty("type_line").GetString(),
                OracleText = cardData.GetProperty("oracle_text").GetString(),
                Power = cardData.GetProperty("power").GetString(),
                Toughness = cardData.GetProperty("toughness").GetString(),
                Rarity = cardData.GetProperty("rarity").GetString(),
                Artist = cardData.GetProperty("artist").GetString(),
                ImageUrl = cardData.GetProperty("image_uris").GetProperty("normal").GetString(),
                ScryfallUri = cardData.GetProperty("scryfall_uri").GetString(),
                CreatedAt = DateTime.UtcNow
            };

            cardList.Add(card);
        }

        Console.WriteLine($"Parsed {cardList.Count} cards.");
        return cardList;
    }


    // Get the next page URL from the Scryfall API's response for pagination
    private static string GetNextPageUrl(string response)
    {
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response);
        return jsonResponse.GetProperty("next_page").GetString();
    }
}

//public class HangfireJobActivator : JobActivator
//{
//    private readonly IServiceProvider _serviceProvider;

//    public HangfireJobActivator(IServiceProvider serviceProvider)
//    {
//        _serviceProvider = serviceProvider;
//    }

//    public override object ActivateJob(Type jobType)
//    {
//        return _serviceProvider.GetRequiredService(jobType);
//    }
//}
