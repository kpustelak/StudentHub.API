using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StudentHub.API.Context;
using System.Globalization;
using System.Text;
using dotenv.net;
using StudentHub.API.Interface;
using StudentHub.API.Service;
using StudentHub.API.Repositories;
using StudentHub.API.Helpers;
using StudentHub.API.Services;
using StudentHub.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

var discordClientId = Environment.GetEnvironmentVariable("DISCORD_ID") ?? throw new InvalidOperationException("DISCORD_ID environment variable is not set.");
var discordClientSecret = Environment.GetEnvironmentVariable("DISCORD_SECRET") ?? throw new InvalidOperationException("DISCORD_SECRET environment variable is not set.");
var jwtSecret =  Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new InvalidOperationException("JWT_SECRET environment variable is not set.");

builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddSqlite<StudentHubDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IGroupAccessService, GroupAccessService>();
builder.Services.AddScoped<IStudentGroupRepository, StudentGroupRepository>();
builder.Services.AddScoped<IStudentGroupService, StudentGroupService>();
builder.Services.AddScoped<LoginHelper>();

builder.Services.AddSignalR();

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ChatClient", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });
});

// Configure authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie("External", options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.Cookie.Name = "ExternalCookie";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = false;
    })
    .AddJwtBearer("Bearer",options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token) && context.Request.Path.StartsWithSegments("/chat"))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    })
    .AddDiscord(options =>
    {
        options.ClientId = discordClientId;
        options.ClientSecret = discordClientSecret;

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
// SignalR hub mapping

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("ChatClient");
app.MapHub<ChatHub>("/chat");

app.MapControllers();

app.Run();

public partial class Program { }
