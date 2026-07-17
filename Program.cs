using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StudentHub.API.Context;
using System.Globalization;
using System.Text;
using dotenv.net;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSqlite<StudentHubDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Authentication setup
var key = "this is my custom Secret key for authentication";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie("External",options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.Cookie.Name = "ExternalCookie";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = false;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    })
     .AddDiscord(options =>
     {
         options.ClientId = builder.Configuration["DISCORD_ID"] ?? throw new Exception("Missing variable");
         options.ClientSecret = builder.Configuration["DISCORD_SECRET"] ?? throw new Exception("Missing variable");

         options.SignInScheme = "External";

         options.ClaimActions.MapJsonKey("urn:discord:id", "id");
         options.ClaimActions.MapJsonKey("urn:discord:username", "username");
         options.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
             string.Format(
                 CultureInfo.InvariantCulture,
                 "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
                 user.GetString("id"),
                 user.GetString("avatar"),
                 user.GetString("avatar")?.StartsWith("a_") == true ? "gif" : "png"));
     });

builder.Services.AddAuthorization();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
