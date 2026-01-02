using FIRST.Data;
using FIRST.DTOs.Teachers;
using FIRST.Models;
using Microsoft.EntityFrameworkCore;

namespace FIRST.Repositories.Teachers;

public class TeacherRepository : ITeacherRepository
{
    private readonly AppDbContext _db;

    public TeacherRepository(AppDbContext db) => _db = db;

    public async Task<List<TeacherDto>> GetAllTeachersAsync()
    {
        return await _db.Teachers
            .Select(t => new TeacherDto
            {
                Id = t.Id,
                Nom = t.Nom,
                Prenom = t.Prenom,
                Cin = t.Cin,
                DateNaissance = t.DateNaissance
            })
            .ToListAsync();
    }

    public async Task<TeacherDto?> GetTeacherDtoByIdAsync(int teacherId)
    {
        return await _db.Teachers
            .Where(t => t.Id == teacherId)
            .Select(t => new TeacherDto
            {
                Id = t.Id,
                Nom = t.Nom,
                Prenom = t.Prenom,
                Cin = t.Cin,
                DateNaissance = t.DateNaissance
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Teacher?> GetTeacherEntityByIdAsync(int teacherId)
    {
        return await _db.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
    }
}
