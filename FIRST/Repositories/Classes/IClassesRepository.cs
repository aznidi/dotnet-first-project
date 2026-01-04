using FIRST.DTOs.Classes;
using FIRST.Models.Classes;
namespace FIRST.Repositories.Classes
{
    public interface IClassesRepository
    {
        Task<List<ClassDto>> GetClassesAsync();

        Task<ClassDto?> GetClassByIdAsync(int id);

        Task<Class?> GetClassEntityByIdAsync(int id);
    }
}