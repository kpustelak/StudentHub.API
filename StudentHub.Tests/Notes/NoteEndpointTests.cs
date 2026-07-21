using System.Net;
using System.Net.Http.Json;
using StudentHub.API.Models;
using StudentHub.API.Models.Dtos;
using StudentHub.Tests.Infrastructure;

namespace StudentHub.Tests.Notes;

[Collection("Integration")]
public class NoteEndpointTests
{
    private readonly StudentHubApplicationFixture _fixture;

    public NoteEndpointTests(StudentHubApplicationFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateNote_AsGroupMember_WithFile_ReturnsNoteWithFile()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        using var form = TestHttpClientFactory.BuildNoteForm(
            "Lecture notes",
            "Chapter 1 summary",
            _fixture.Seed.Group.Id,
            fileBytes: "%PDF-1.4 test content"u8.ToArray(),
            fileName: "notes.pdf");

        var response = await client.PostAsync("/api/note", form);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<NoteDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.True(body.Status);
        Assert.Single(body.Data.Files);
        Assert.Equal("notes.pdf", body.Data.Files[0].Title);
        Assert.StartsWith("/uploads/notes/", body.Data.Files[0].Url);
        Assert.Contains(_fixture.Seed.Member.Id, body.Data.ContributorIds);
    }

    [Fact]
    public async Task CreateNote_AsNonMember_ReturnsBadRequest()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Outsider);

        using var form = TestHttpClientFactory.BuildNoteForm(
            "Blocked note",
            "Should fail",
            _fixture.Seed.Group.Id);

        var response = await client.PostAsync("/api/note", form);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<NoteDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.False(body.Status);
        Assert.Contains("not a member", body.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateNote_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = _fixture.Factory.CreateClient();
        using var form = TestHttpClientFactory.BuildNoteForm(
            "Unauthorized",
            "No token",
            _fixture.Seed.Group.Id);

        var response = await client.PostAsync("/api/note", form);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetNoteById_IncrementsViewCount()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        using var createForm = TestHttpClientFactory.BuildNoteForm(
            "View count note",
            "Testing views",
            _fixture.Seed.Group.Id);

        var createResponse = await client.PostAsync("/api/note", createForm);
        var created = await createResponse.Content.ReadFromJsonAsync<ResponseModel<NoteDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(created);

        var firstGet = await client.GetAsync($"/api/note/{created.Data.Id}");
        var firstBody = await firstGet.Content.ReadFromJsonAsync<ResponseModel<NoteDto>>(
            TestHttpClientFactory.JsonOptions);

        var secondGet = await client.GetAsync($"/api/note/{created.Data.Id}");
        var secondBody = await secondGet.Content.ReadFromJsonAsync<ResponseModel<NoteDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(firstBody);
        Assert.NotNull(secondBody);
        Assert.Equal(1, firstBody.Data.ViewCount);
        Assert.Equal(2, secondBody.Data.ViewCount);
    }

    [Fact]
    public async Task GetNotesByGroup_ReturnsGroupNotes()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        using var form = TestHttpClientFactory.BuildNoteForm(
            $"Group list note {Guid.NewGuid():N}",
            "Listed by group",
            _fixture.Seed.Group.Id);

        await client.PostAsync("/api/note", form);

        var response = await client.GetAsync($"/api/note/group/{_fixture.Seed.Group.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ResponseModel<List<NoteDto>>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(body);
        Assert.True(body.Status);
        Assert.NotEmpty(body.Data);
        Assert.All(body.Data, note =>
            Assert.Contains(_fixture.Seed.Group.Id, note.StudentGroupIds));
    }

    [Fact]
    public async Task UpdateNote_AsContributor_UpdatesFields()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        using var createForm = TestHttpClientFactory.BuildNoteForm(
            "Before update",
            "Old description",
            _fixture.Seed.Group.Id);

        var createResponse = await client.PostAsync("/api/note", createForm);
        var created = await createResponse.Content.ReadFromJsonAsync<ResponseModel<NoteDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(created);

        using var updateForm = new MultipartFormDataContent
        {
            { new StringContent("After update"), "Name" },
            { new StringContent("New description"), "Description" },
            { new StringContent("false"), "IsActive" }
        };

        var updateResponse = await client.PutAsync($"/api/note/{created.Data.Id}", updateForm);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<ResponseModel<NoteDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(updated);
        Assert.Equal("After update", updated.Data.Name);
        Assert.Equal("New description", updated.Data.Description);
        Assert.False(updated.Data.IsActive);
    }

    [Fact]
    public async Task DeleteNote_AsContributor_SoftDeletesNote()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        using var createForm = TestHttpClientFactory.BuildNoteForm(
            "To delete",
            "Will be soft deleted",
            _fixture.Seed.Group.Id);

        var createResponse = await client.PostAsync("/api/note", createForm);
        var created = await createResponse.Content.ReadFromJsonAsync<ResponseModel<NoteDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(created);

        var deleteResponse = await client.DeleteAsync($"/api/note/{created.Data.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/note/{created.Data.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task ReportNote_SetsIsReportedFlag()
    {
        using var client = TestHttpClientFactory.CreateAuthenticatedClient(
            _fixture.Factory,
            _fixture.Seed.Member);

        using var createForm = TestHttpClientFactory.BuildNoteForm(
            "Report me",
            "Report test",
            _fixture.Seed.Group.Id);

        var createResponse = await client.PostAsync("/api/note", createForm);
        var created = await createResponse.Content.ReadFromJsonAsync<ResponseModel<NoteDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(created);

        var reportResponse = await client.PostAsync($"/api/note/{created.Data.Id}/report", null);
        Assert.Equal(HttpStatusCode.OK, reportResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/note/{created.Data.Id}");
        var note = await getResponse.Content.ReadFromJsonAsync<ResponseModel<NoteDto>>(
            TestHttpClientFactory.JsonOptions);

        Assert.NotNull(note);
        Assert.True(note.Data.IsReported);
    }
}
