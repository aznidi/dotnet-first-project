using FIRST.Data;
using FIRST.DTOs.Subjects;
using FIRST.Models;
using Microsoft.EntityFrameworkCore;

namespace FIRST.Repositories.Subjects;

public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _db;
    public SubjectRepository(AppDbContext db) => _db = db;

    public async Task<List<SubjectDto>> GetAllAsync()
    {
        return await _db.Subjects
            .OrderBy(s => s.Name)
            .Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                IsActive = s.IsActive
            })
            .ToListAsync();
    }

    public async Task<SubjectDto?> GetDtoByIdAsync(int id)
    {
        return await _db.Subjects
            .Where(s => s.Id == id)
            .Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                IsActive = s.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Subject?> GetEntityByIdAsync(int id)
        => await _db.Subjects.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var query = _db.Subjects.AsQueryable().Where(s => s.Code == code);
        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId.Value);

        return await query.AnyAsync();
    }
}
