using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Context
{
    public class StudentHubDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<Semester> Semesters => Set<Semester>();
        public DbSet<StudentGroup> StudentGroups => Set<StudentGroup>();

        public StudentHubDbContext(DbContextOptions<StudentHubDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.DiscordId).IsUnique();

                entity.HasMany(x => x.Notes)
                    .WithMany(x => x.Contributors);

                entity.HasMany(x => x.Semesters)
                    .WithMany(x => x.Students);

                entity.HasMany(x => x.StudentGroups)
                    .WithMany(x => x.Members);
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasMany(x => x.StudentGroups)
                    .WithOne(x => x.Semester)
                    .HasForeignKey(x => x.SemesterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<StudentGroup>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasMany(x => x.Notes)
                    .WithMany(x => x.StudentGroups);
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.FileUrls)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());
            });
        }
    }
}
