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
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Models.Entities.File> Files => Set<Models.Entities.File>();

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

                entity.HasMany(x => x.Messages)
                    .WithOne(x => x.Student);
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

                entity.HasMany(x => x.Files)
                    .WithOne(x => x.Note)
                    .HasForeignKey(x => x.NoteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.StudentGroup)
                    .WithMany(x => x.Messages)
                    .HasForeignKey(x => x.StudentGroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Student)
                    .WithMany(x => x.Messages)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
