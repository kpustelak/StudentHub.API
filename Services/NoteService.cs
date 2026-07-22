using StudentHub.API.Interface;
using StudentHub.API.Models.Dtos;
using StudentHub.API.Models.Entities;
using FileEntity = StudentHub.API.Models.Entities.File;

namespace StudentHub.API.Services
{
    public class NoteService : INoteService
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".docx", ".png", ".jpg", ".jpeg"
        };

        private const long MaxFileSizeBytes = 20 * 1024 * 1024;

        private readonly INoteRepository _noteRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStudentGroupRepository _helperRepository;
        private readonly IWebHostEnvironment _environment;

        public NoteService(
            INoteRepository noteRepository,
            IUserRepository userRepository,
            IWebHostEnvironment environment,
            IStudentGroupRepository helperRepository)
        {
            _noteRepository = noteRepository;
            _userRepository = userRepository;
            _environment = environment;
            _helperRepository = helperRepository;
        }

        public async Task<NoteDto> CreateNoteAsync(AddNoteDto dto, string userId)
        {
            var group = await _helperRepository.GetByIdWithMembersAsync(dto.StudentGroupId)
                ?? throw new ArgumentException("Student group not found.");

            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentException("User not found.");

            if (group.Members.All(m => m.Id != userId))
            {
                throw new InvalidOperationException("User is not a member of this group.");
            }

            var note = new Note
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true,
                IsDeleted = false,
                IsReported = false,
                ViewCount = 0
            };

            note.Contributors.Add(user);
            note.StudentGroups.Add(group);

            if (dto.File is not null)
            {
                note.Files.Add(await CreateFileEntityAsync(dto.File, note.Id));
            }

            var created = await _noteRepository.CreateNoteAsync(note);
            return MapToDto(created);
        }

        public async Task<NoteDto> GetNoteByIdAsync(string id)
        {
            var note = await _noteRepository.GetNoteByIdAsync(id)
                ?? throw new ArgumentException("Note not found.");

            note.ViewCount++;
            await _noteRepository.UpdateNoteAsync(note);

            return MapToDto(note);
        }

        public async Task<List<NoteDto>> GetNotesByGroupIdAsync(string groupId)
        {
            var notes = await _noteRepository.GetNotesByGroupIdAsync(groupId);
            return notes.Select(MapToDto).ToList();
        }

        public async Task<NoteDto> UpdateNoteAsync(string id, EditNoteDto dto, string userId)
        {
            var note = await _noteRepository.GetNoteByIdAsync(id)
                ?? throw new ArgumentException("Note not found.");

            EnsureContributor(note, userId);

            note.Name = dto.Name;
            note.Description = dto.Description;
            note.IsActive = dto.IsActive;

            if (dto.File is not null)
            {
                note.Files.Add(await CreateFileEntityAsync(dto.File, note.Id));
            }

            var updated = await _noteRepository.UpdateNoteAsync(note);
            return MapToDto(updated);
        }

        public async Task DeleteNoteAsync(string id, string userId)
        {
            var note = await _noteRepository.GetNoteByIdAsync(id)
                ?? throw new ArgumentException("Note not found.");

            EnsureContributor(note, userId);

            note.IsDeleted = true;
            note.IsActive = false;
            await _noteRepository.UpdateNoteAsync(note);
        }

        public async Task ReportNoteAsync(string id)
        {
            var note = await _noteRepository.GetNoteByIdAsync(id)
                ?? throw new ArgumentException("Note not found.");

            note.IsReported = true;
            await _noteRepository.UpdateNoteAsync(note);
        }

        private async Task<FileEntity> CreateFileEntityAsync(IFormFile file, string noteId)
        {
            var url = await SaveFileAsync(file);
            return new FileEntity
            {
                Id = Guid.NewGuid().ToString(),
                Title = file.FileName,
                Url = url,
                Size = file.Length,
                NoteId = noteId
            };
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new ArgumentException("File is empty.");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                throw new ArgumentException("File exceeds the 20 MB size limit.");
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException("File type is not allowed.");
            }

            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "notes");
            Directory.CreateDirectory(uploadsRoot);

            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var physicalPath = Path.Combine(uploadsRoot, storedFileName);

            await using var stream = new FileStream(physicalPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/notes/{storedFileName}";
        }

        private static void EnsureContributor(Note note, string userId)
        {
            if (note.Contributors.All(c => c.Id != userId))
            {
                throw new InvalidOperationException("Only contributors can modify this note.");
            }
        }

        private static NoteDto MapToDto(Note note)
        {
            return new NoteDto(
                note.Id,
                note.Name,
                note.Description,
                note.Files.Select(f => new FileDto(f.Id, f.Title, f.Url, f.Size)).ToList(),
                note.IsActive,
                note.IsReported,
                note.ViewCount,
                note.Contributors.Select(c => c.Id).ToList(),
                note.StudentGroups.Select(g => g.Id).ToList()
            );
        }
    }
}
