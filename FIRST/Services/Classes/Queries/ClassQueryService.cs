using FIRST.DTOs.Classes;
using FIRST.Models.Classes;
using FIRST.Repositories.Classes;
namespace FIRST.Services.Classes.Queries
{
    public class ClassQueryService
    {
        private readonly IClassesRepository _r;

        public ClassQueryService(IClassesRepository classesRepository)
        {
            _r = classesRepository;
        }

        public async Task<List<ClassDto>> GetClassesAsync()
        {
            return await _r.GetClassesAsync();
        }

        public async Task<ClassDto?> GetClassByIdAsync(int id)
        {
            return await _r.GetClassByIdAsync(id);
        }
        
    }
}