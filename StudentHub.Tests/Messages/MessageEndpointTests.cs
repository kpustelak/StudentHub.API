using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using StudentHub.API.Models;
using StudentHub.API.Models.Dtos;
using StudentHub.Tests.Infrastructure;

namespace StudentHub.Tests.Messages;

[Collection("Integration")]
public class MessageEndpointTests
{
    private readonly StudentHubApplicationFixture _fixture;

    public MessageEndpointTests(StudentHubApplicationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetMessages_AsGroupMember_ReturnsEmptyListInitially()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        var response = await client.GetAsync($"/api/message/group/{_fixture.Seed.Group.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<List<MessageDto>>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.True(body.Status);
        Assert.NotNull(body.Data);
    }

    [Fact]
    public async Task GetMessages_AfterSendViaHub_ReturnsPersistedMessage()
    {
        const string content = "Hello from integration test";

        HubConnection? connection = null;
        try
        {
            connection = await TestSignalRConnectionFactory.ConnectAsync(
                _fixture.Factory,
                _fixture.Seed.Member);

            await connection.InvokeAsync("JoinGroup", _fixture.Seed.Group.Id);
            await connection.InvokeAsync("SendMessage", _fixture.Seed.Group.Id, content);

            using var client = TestHttpClientFactory.CreateAuthenticatedClient(
                _fixture.Factory,
                _fixture.Seed.Member);

            var response = await client.GetAsync($"/api/message/group/{_fixture.Seed.Group.Id}");
            var body = await response.Content.ReadFromJsonAsync<ResponseModel<List<MessageDto>>>(
                TestHttpClientFactory.JsonOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(body);
            Assert.Contains(body.Data, message =>
                message.Content == content &&
                message.UserName == _fixture.Seed.Member.Username);
        }
        finally
        {
            if (connection is not null)
            {
                await connection.DisposeAsync();
            }
        }
    }

    [Fact]
    public async Task GetMessages_AsNonMember_ReturnsBadRequest()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Outsider);

        var response = await client.GetAsync($"/api/message/group/{_fixture.Seed.Group.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<List<MessageDto>>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.False(body.Status);
        Assert.Contains("not a member", body.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetMessages_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = _fixture.Factory.CreateClient();

        var response = await client.GetAsync($"/api/message/group/{_fixture.Seed.Group.Id}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
