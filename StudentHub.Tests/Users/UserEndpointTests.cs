using System.Net;
using System.Net.Http.Json;
using StudentHub.API.Models;
using StudentHub.API.Models.Dtos;
using StudentHub.Tests.Infrastructure;

namespace StudentHub.Tests.Users;

[Collection("Integration")]
public class UserEndpointTests
{
    private readonly StudentHubApplicationFixture _fixture;

    public UserEndpointTests(StudentHubApplicationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetMe_ReturnsAuthenticatedUser()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        var response = await client.GetAsync("/api/user/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<UserDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.True(body.Status);
        Assert.Equal(_fixture.Seed.Member.Id, body.Data.Id);
        Assert.Equal("member", body.Data.Username);
    }

    [Fact]
    public async Task GetMe_WithoutToken_ReturnsUnauthorized()
    {
        using var client = _fixture.Factory.CreateClient();

        var response = await client.GetAsync("/api/user/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
