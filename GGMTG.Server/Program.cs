using GGMTG.Services; // Make sure the namespace matches the one in your service.
using Microsoft.EntityFrameworkCore;
using System.Xml;
using GGMTG.Server.Models;
using GGMTG.Server;
using GGMTG.Server.Repositories;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GGMTG.Server.Controllers;
using GGMTG.Server.Services;


var doc = new XmlDocument();
string env = Environment.GetEnvironmentVariable("JYGGALAG") ?? throw new Exception("JYGGALAG variable not set!");
doc.Load(env);

if (doc.DocumentElement is null)
{
    throw new Exception("connection.xml not found");
}

XmlNode node = doc.DocumentElement.SelectSingleNode("/Connection")!;

string connString
    = "Server=" + node["Server"]!.InnerText
    + ";Database=" + node["Database"]!.InnerText
    + ";User Id=" + node["UserName"]!.InnerText
    + ";Password=" + node["Password"]!.InnerText;

if (node["Encrypt"] != null)
{
    connString += ";Encrypt=" + node["Encrypt"]!.InnerText;
}
if (node["TrustServerCertificate"] != null)
{
    connString += ";TrustServerCertificate=" + node["TrustServerCertificate"]!.InnerText;
}

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("https://localhost:7057",
                                "https://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
        });
});
builder.Services
    .AddTransient<IAccountRepository, AccountRepository>()
    .AddTransient<ISecurityService, SecurityService>();

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Context>(opt =>
    opt.UseSqlServer(connString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
c.SwaggerDoc("v1", new OpenApiInfo
{
    Version = "v1",
    Title = "Code Blueprints API",
    Description = "API Description",
});

c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
    new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            new List<string>()
        }
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
                                                                           throw new InvalidOperationException("Could not find JWT configuration")))
    };
});


//builder.Services.AddRazorPages();
//// Register CardScrapeService with HttpClient (if you have a constructor that needs HttpClient).
//builder.Services.AddHttpClient<CardScrapeService>();

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetRequiredService<Context>();
dbContext.Database.EnsureCreated();

//app.UseDefaultFiles();
//app.UseStaticFiles();

app.Use(async (context, next) =>
{
    string? accessToken;
    if (context.Request.Cookies.TryGetValue("code-blueprints-access-token", out accessToken))
    {
        context.Request.Headers.Authorization = "Bearer " + accessToken;
    }
    await next(context);
});



app.UseDefaultFiles();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");
app.Run();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();
//app.MapFallbackToFile("/index.html");

//app.Run();
