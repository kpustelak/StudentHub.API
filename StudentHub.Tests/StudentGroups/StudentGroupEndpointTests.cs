using System.Net;
using System.Net.Http.Json;
using StudentHub.API.Models;
using StudentHub.API.Models.Dtos;
using StudentHub.Tests.Infrastructure;

namespace StudentHub.Tests.StudentGroups;

[Collection("Integration")]
public class StudentGroupEndpointTests
{
    private readonly StudentHubApplicationFixture _fixture;

    public StudentGroupEndpointTests(StudentHubApplicationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetMyGroups_AsMember_ReturnsGroups()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        var response = await client.GetAsync("/api/studentgroup/my");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<List<StudentGroupDto>>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.True(body.Status);
        Assert.Contains(body.Data, g => g.Id == _fixture.Seed.Group.Id);
    }

    [Fact]
    public async Task GetSemesterGroups_ReturnsGroupsInSemester()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        var response = await client.GetAsync($"/api/studentgroup/semester/{_fixture.Seed.Semester.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<List<StudentGroupDto>>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.True(body.Status);
        Assert.Contains(body.Data, g => g.Id == _fixture.Seed.Group.Id);
    }

    [Fact]
    public async Task GetById_ReturnsGroup()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        var response = await client.GetAsync($"/api/studentgroup/{_fixture.Seed.Group.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<StudentGroupDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.Equal(_fixture.Seed.Group.Name, body.Data.Name);
    }

    [Fact]
    public async Task CreateGroup_AddsCreatorAsMember()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Outsider);

        var response = await client.PostAsJsonAsync(
            $"/api/studentgroup/semester/{_fixture.Seed.Semester.Id}",
            new AddStudentGroupDto { Name = "New Group", Description = "Created in test" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<StudentGroupDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.True(body.Status);
        Assert.Equal("New Group", body.Data.Name);

        var myGroups = await client.GetAsync("/api/studentgroup/my");
        var myBody = await myGroups.Content.ReadFromJsonAsync<ResponseModel<List<StudentGroupDto>>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(myBody);
        Assert.Contains(myBody.Data, g => g.Id == body.Data.Id);
    }

    [Fact]
    public async Task JoinGroup_AsNonMember_AddsUser()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Outsider);

        var response = await client.PostAsync($"/api/studentgroup/join/{_fixture.Seed.Group.Id}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var myGroups = await client.GetAsync("/api/studentgroup/my");
        var myBody = await myGroups.Content.ReadFromJsonAsync<ResponseModel<List<StudentGroupDto>>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(myBody);
        Assert.Contains(myBody.Data, g => g.Id == _fixture.Seed.Group.Id);
    }

    [Fact]
    public async Task JoinGroup_WhenAlreadyMember_ReturnsBadRequest()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        var response = await client.PostAsync($"/api/studentgroup/join/{_fixture.Seed.Group.Id}", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LeaveGroup_RemovesUser()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Outsider);

        await client.PostAsync($"/api/studentgroup/join/{_fixture.Seed.Group.Id}", null);

        var response = await client.PostAsync($"/api/studentgroup/leave/{_fixture.Seed.Group.Id}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var myGroups = await client.GetAsync("/api/studentgroup/my");
        var myBody = await myGroups.Content.ReadFromJsonAsync<ResponseModel<List<StudentGroupDto>>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(myBody);
        Assert.DoesNotContain(myBody.Data, g => g.Id == _fixture.Seed.Group.Id);
    }

    [Fact]
    public async Task GetMyGroups_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = _fixture.Factory.CreateClient();

        var response = await client.GetAsync("/api/studentgroup/my");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
