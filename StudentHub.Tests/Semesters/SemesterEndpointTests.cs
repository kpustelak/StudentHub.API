using System.Net;
using System.Net.Http.Json;
using StudentHub.API.Models;
using StudentHub.API.Models.Dtos;
using StudentHub.Tests.Infrastructure;

namespace StudentHub.Tests.Semesters;

[Collection("Integration")]
public class SemesterEndpointTests
{
    private readonly StudentHubApplicationFixture _fixture;

    public SemesterEndpointTests(StudentHubApplicationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateSemester_ReturnsCreatedSemester()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        var dto = new AddSemesterDto(
            "New Semester",
            "NS",
            "Created in test",
            new DateOnly(2026, 10, 1),
            new DateOnly(2027, 1, 31));

        var response = await client.PostAsJsonAsync("/api/semester", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<SemesterDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.True(body.Status);
        Assert.Equal("New Semester", body.Data.Title);
    }

    [Fact]
    public async Task ListSemesters_ReturnsAtLeastOneSemester()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        var response = await client.GetAsync("/api/semester");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<IEnumerable<SemesterDto>>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.True(body.Status);
        Assert.NotEmpty(body.Data!);
    }

    [Fact]
    public async Task JoinSemester_AddsUserToSemester()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Outsider);

        var joinResponse = await client.PostAsync(
            $"/api/semester/join/{_fixture.Seed.Semester.Id}",
            null);

        Assert.Equal(HttpStatusCode.OK, joinResponse.StatusCode);

        var meResponse = await client.GetAsync("/api/user/me");
        var me = await meResponse.Content.ReadFromJsonAsync<ResponseModel<UserDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(me);
        Assert.Contains(me.Data.SemestersDto!, s => s.Id == _fixture.Seed.Semester.Id);
    }
}
