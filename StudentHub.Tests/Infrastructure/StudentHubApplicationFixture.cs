using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentHub.API.Context;
using StudentHub.API.Models.Entities;

namespace StudentHub.Tests.Infrastructure;

public record TestSeed(
    User Member,
    User Outsider,
    Semester Semester,
    StudentGroup Group);

public static class TestDataSeeder
{
    public static async Task<TestSeed> SeedAsync(StudentHubDbContext db)
    {
        if (await db.Users.AnyAsync())
        {
            var member = await db.Users.FirstAsync(u => u.Username == "member");
            var outsider = await db.Users.FirstAsync(u => u.Username == "outsider");
            var semester = await db.Semesters.FirstAsync();
            var group = await db.StudentGroups
                .Include(g => g.Members)
                .FirstAsync();

            return new TestSeed(member, outsider, semester, group);
        }

        var memberUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            DiscordId = "discord-member",
            Username = "member",
            AvatarUrl = "https://cdn.example/avatar.png"
        };

        var outsiderUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            DiscordId = "discord-outsider",
            Username = "outsider",
            AvatarUrl = string.Empty
        };

        var semesterEntity = new Semester
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Test Semester",
            ShortTitle = "TS",
            Description = "Integration test semester",
            StartDate = new DateOnly(2026, 3, 1),
            EndDate = new DateOnly(2026, 6, 30)
        };

        var groupEntity = new StudentGroup
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Group",
            Description = "Group for note tests",
            SemesterId = semesterEntity.Id,
            Semester = semesterEntity
        };

        groupEntity.Members.Add(memberUser);
        semesterEntity.Students.Add(memberUser);

        db.Users.AddRange(memberUser, outsiderUser);
        db.Semesters.Add(semesterEntity);
        db.StudentGroups.Add(groupEntity);
        await db.SaveChangesAsync();

        return new TestSeed(memberUser, outsiderUser, semesterEntity, groupEntity);
    }
}

public class StudentHubApplicationFixture : IAsyncLifetime
{
    public StudentHubWebApplicationFactory Factory { get; } = new();
    public TestSeed Seed { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudentHubDbContext>();
        await db.Database.MigrateAsync();
        Seed = await TestDataSeeder.SeedAsync(db);
    }

    public Task DisposeAsync()
    {
        Factory.Dispose();
        return Task.CompletedTask;
    }
}

[CollectionDefinition("Integration")]
public class IntegrationCollection : ICollectionFixture<StudentHubApplicationFixture>;
