using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using StudentHub.API.Interface;
using StudentHub.API.Models.Entities;

namespace StudentHub.Tests.Infrastructure;

public static class TestHttpClientFactory
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static HttpClient CreateAuthenticatedClient(
        StudentHubWebApplicationFactory factory,
        User user)
    {
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
        var token = jwtService.GenerateToken(user);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    public static MultipartFormDataContent BuildNoteForm(
        string name,
        string description,
        string studentGroupId,
        byte[]? fileBytes = null,
        string? fileName = null)
    {
        var form = new MultipartFormDataContent
        {
            { new StringContent(name), "Name" },
            { new StringContent(description), "Description" },
            { new StringContent(studentGroupId), "StudentGroupId" }
        };

        if (fileBytes is not null && fileName is not null)
        {
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            form.Add(fileContent, "File", fileName);
        }

        return form;
    }
}
