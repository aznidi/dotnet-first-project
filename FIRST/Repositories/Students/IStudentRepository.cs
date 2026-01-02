using FIRST.DTOs;
using FIRST.Models; 

namespace FIRST.Repositories.Students
{
    public interface IStudentRepository
    {
        Task<List<StudentDto>> GetAllStudentsAsync();

        Task<StudentDto?> GetStudentDtoById(int StudentId);

        Task<Student?> GetStudentEntityByIdAsync(int studentId);
    }
}