using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using StudentHub.API.Interface;
using StudentHub.API.Models.Entities;

namespace StudentHub.Tests.Infrastructure;

public static class TestSignalRConnectionFactory
{
    public static async Task<HubConnection> ConnectAsync(
        StudentHubWebApplicationFactory factory,
        User? user = null)
    {
        var hubUri = BuildHubUri(factory, user);

        var connection = new HubConnectionBuilder()
            .WithUrl(hubUri, options =>
            {
                options.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler();
            })
            .Build();

        await connection.StartAsync();
        return connection;
    }

    private static Uri BuildHubUri(StudentHubWebApplicationFactory factory, User? user)
    {
        var baseAddress = factory.Server.BaseAddress
            ?? throw new InvalidOperationException("Test server base address is not configured.");

        var hubUrl = new Uri(baseAddress, "chat");

        if (user is null)
        {
            return hubUrl;
        }

        using var scope = factory.Services.CreateScope();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
        var token = jwtService.GenerateToken(user);

        var builder = new UriBuilder(hubUrl)
        {
            Query = $"access_token={Uri.EscapeDataString(token)}"
        };

        return builder.Uri;
    }
}
