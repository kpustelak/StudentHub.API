using StudentHub.API.Interface;
using StudentHub.API.Models.Dtos;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _repository;
        private readonly IUserRepository _userRepository;
        public SemesterService(ISemesterRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<Semester> AddUserToSemester(string userId, string semesterId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            var semester = await _repository.GetSemesterByIdAsync(semesterId);

            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(semester);

            if (user!.Semesters.FirstOrDefault(x => x.Id == semesterId) is not null) throw new Exception("User is already signed into semester");

            var newSemester = await _repository.AddUserToSemesterAsync(user, semester!);
            return newSemester;
        }

        public async Task<Semester> CreateSemesterAsync(AddSemesterDto dto)
        {
            var semester = await _repository.CreateSemesterAsync(new Semester
                {
                    Id = Guid.NewGuid().ToString(),
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    ShortTitle = dto.ShortTitle,
                    Description = dto.Description,
                    Title = dto.Title
                });
                return semester;
        }

        public async Task DeleteSemesterAsync(string id)
        {
            var semester = await _repository.GetSemesterByIdAsync(id);
            ArgumentNullException.ThrowIfNull(semester);
            await _repository.DeleteSemesterAsync(semester);
        }

        public async Task DeleteUserFromSemester(string userId, string semesterId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            var semester = await _repository.GetSemesterByIdAsync(semesterId);
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(semester);
            if (user!.Semesters.FirstOrDefault(x => x.Id == semesterId) is null) throw new Exception("User is not signed into semester");
            await _repository.DeleteUserFromSemester(user, semester);
        }

        public async Task<Semester> GetSemesterByIdAsync(string id)
        {
            return await _repository.GetSemesterByIdAsync(id) ?? throw new ArgumentException("No matching semester.");

        }

        public async Task<List<Semester>> GetSemestersAsync()
        {
            return await _repository.GetSemestersAsync();
        }

        public async Task<Semester> UpdateSemesterAsync(Semester dto, string id)
        {
            var semester = await _repository.GetSemesterByIdAsync(id);
            ArgumentNullException.ThrowIfNull(semester);
            return await _repository.UpdateSemesterAsync(new Semester
                {
                    Id = id,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    ShortTitle = dto.ShortTitle,
                    Description = dto.Description,
                    Title = dto.Title
                });
        }
    }
}
