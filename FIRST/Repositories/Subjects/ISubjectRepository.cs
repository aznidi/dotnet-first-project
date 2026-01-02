using FIRST.DTOs.Subjects;
using FIRST.Models;

namespace FIRST.Repositories.Subjects;

public interface ISubjectRepository
{
    Task<List<SubjectDto>> GetAllAsync();
    Task<SubjectDto?> GetDtoByIdAsync(int id);
    Task<Subject?> GetEntityByIdAsync(int id);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}
