namespace StudentHub.API.Models.Dtos
{
    public class AddStudentGroupDto
    {

        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
