using GGMTG.Services;
using Microsoft.EntityFrameworkCore;
using GGMTG.Server.Models;
using GGMTG.Server;
using GGMTG.Server.Repositories;
using Microsoft.OpenApi.Models;
using System.Reflection;
using GGMTG.Server.Controllers;
using GGMTG.Server.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    // Disable SSL certificate validation
    System.Net.ServicePointManager.ServerCertificateValidationCallback =
        (sender, certificate, chain, sslPolicyErrors) => true;
}
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:7057",  // Your frontend server URL
                                "http://localhost:52999") // Your frontend server URL (this should match your React app's URL)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services
    .AddTransient<IAccountRepository, AccountRepository>()
    .AddTransient<ISecurityService, SecurityService>();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddControllersWithViews();

// Configure SQLite
builder.Services.AddDbContext<Context>(opt =>
    opt.UseSqlite("Data Source=C:\\Users\\Chance Barimbao\\Documents\\testMTG.db"));

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

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetRequiredService<Context>();
dbContext.Database.EnsureCreated();

app.UseDefaultFiles();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();

app.UseRouting(); // Ensure routing is enabled

app.UseAuthorization();

// ADD SWAGGER MIDDLEWARE BEFORE `app.MapControllers()`
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Blueprints API V1");
    c.RoutePrefix = "swagger"; // Ensures Swagger UI is accessible at `/swagger`
});

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
