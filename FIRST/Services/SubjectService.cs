using FIRST.Data;
using FIRST.DTOs.Subjects;
using FIRST.Models;
using FIRST.Repositories.Subjects;

namespace FIRST.Services;

public class SubjectService
{
    private readonly ISubjectRepository _repo;
    private readonly AppDbContext _db;

    public SubjectService(ISubjectRepository repo, AppDbContext db)
    {
        _repo = repo;
        _db = db;
    }

    public Task<List<SubjectDto>> GetAllAsync() => _repo.GetAllAsync();

    public Task<SubjectDto?> GetByIdAsync(int id) => _repo.GetDtoByIdAsync(id);

    public async Task<SubjectDto?> CreateAsync(CreateSubjectDto dto)
    {
        if (await _repo.CodeExistsAsync(dto.Code))
            return null; // controller va retourner 400/409

        var subject = new Subject
        {
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            IsActive = dto.IsActive
        };

        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync();

        return new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Code = subject.Code,
            Description = subject.Description,
            IsActive = subject.IsActive
        };
    }

    public async Task<SubjectDto?> UpdateAsync(int id, UpdateSubjectDto dto)
    {
        var subject = await _repo.GetEntityByIdAsync(id);
        if (subject == null) return null;

        if (await _repo.CodeExistsAsync(dto.Code, excludeId: id))
            return null;

        subject.Name = dto.Name;
        subject.Code = dto.Code;
        subject.Description = dto.Description;
        subject.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();

        return new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Code = subject.Code,
            Description = subject.Description,
            IsActive = subject.IsActive
        };
    }

    public async Task<SubjectDto?> PatchAsync(int id, PatchSubjectDto dto)
    {
        var subject = await _repo.GetEntityByIdAsync(id);
        if (subject == null) return null;

        if (dto.Code != null && await _repo.CodeExistsAsync(dto.Code, excludeId: id))
            return null;

        if (dto.Name != null) subject.Name = dto.Name;
        if (dto.Code != null) subject.Code = dto.Code;
        if (dto.Description != null) subject.Description = dto.Description;
        if (dto.IsActive.HasValue) subject.IsActive = dto.IsActive.Value;

        await _db.SaveChangesAsync();

        return new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Code = subject.Code,
            Description = subject.Description,
            IsActive = subject.IsActive
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subject = await _repo.GetEntityByIdAsync(id);
        if (subject == null) return false;

        _db.Subjects.Remove(subject);
        await _db.SaveChangesAsync();
        return true;
    }
}
