using FIRST.DTOs.Teachers;
using FIRST.Models;

namespace FIRST.Repositories.Teachers;

public interface ITeacherRepository
{
    Task<List<TeacherDto>> GetAllTeachersAsync();
    Task<TeacherDto?> GetTeacherDtoByIdAsync(int teacherId);
    Task<Teacher?> GetTeacherEntityByIdAsync(int teacherId);
}
