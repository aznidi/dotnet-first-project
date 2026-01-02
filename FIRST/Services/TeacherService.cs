using FIRST.Data;
using FIRST.DTOs.Teachers;
using FIRST.Models;
using FIRST.Repositories.Teachers;

namespace FIRST.Services;

public class TeacherService
{
    private readonly ITeacherRepository _repository;
    private readonly AppDbContext _db;

    public TeacherService(ITeacherRepository repository, AppDbContext db)
    {
        _repository = repository;
        _db = db;
    }

    public Task<List<TeacherDto>> GetAllAsync()
        => _repository.GetAllTeachersAsync();

    public Task<TeacherDto?> GetByIdAsync(int id)
        => _repository.GetTeacherDtoByIdAsync(id);

    public async Task<TeacherDto> CreateAsync(CreateTeacherDto dto)
    {
        var teacher = new Teacher
        {
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Cin = dto.Cin,
            DateNaissance = dto.DateNaissance
        };

        _db.Teachers.Add(teacher);
        await _db.SaveChangesAsync();

        return new TeacherDto
        {
            Id = teacher.Id,
            Nom = teacher.Nom,
            Prenom = teacher.Prenom,
            Cin = teacher.Cin,
            DateNaissance = teacher.DateNaissance
        };
    }

    public async Task<TeacherDto?> UpdateAsync(int id, UpdateTeacherDto dto)
    {
        var teacher = await _repository.GetTeacherEntityByIdAsync(id);
        if (teacher == null) return null;

        teacher.Nom = dto.Nom;
        teacher.Prenom = dto.Prenom;
        teacher.Cin = dto.Cin;
        teacher.DateNaissance = dto.DateNaissance;

        await _db.SaveChangesAsync();

        return new TeacherDto
        {
            Id = teacher.Id,
            Nom = teacher.Nom,
            Prenom = teacher.Prenom,
            Cin = teacher.Cin,
            DateNaissance = teacher.DateNaissance
        };
    }

    public async Task<TeacherDto?> PatchAsync(int id, PatchTeacherDto dto)
    {
        var teacher = await _repository.GetTeacherEntityByIdAsync(id);
        if (teacher == null) return null;

        if (dto.Nom != null) teacher.Nom = dto.Nom;
        if (dto.Prenom != null) teacher.Prenom = dto.Prenom;
        if (dto.Cin != null) teacher.Cin = dto.Cin;
        if (dto.DateNaissance != null) teacher.DateNaissance = dto.DateNaissance;

        await _db.SaveChangesAsync();

        return new TeacherDto
        {
            Id = teacher.Id,
            Nom = teacher.Nom,
            Prenom = teacher.Prenom,
            Cin = teacher.Cin,
            DateNaissance = teacher.DateNaissance
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var teacher = await _repository.GetTeacherEntityByIdAsync(id);
        if (teacher == null) return false;

        _db.Teachers.Remove(teacher);
        await _db.SaveChangesAsync();

        return true;
    }
}
