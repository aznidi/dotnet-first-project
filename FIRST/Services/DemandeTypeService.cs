using FIRST.Data;
using FIRST.DTOs;
using FIRST.DTOs.Demandes;
using FIRST.DTOs.DemandesTypes;
using FIRST.Models;
using FIRST.Repositories.Students;
using Microsoft.EntityFrameworkCore;
 
namespace FIRST.Services;
public class DemandeTypeService
{
    private readonly AppDbContext _db;

    public DemandeTypeService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<List<DemandeTypeDto>> GetAllAsync()
    {
        return await _db.DemandeTypes
            .Select(dt => new DemandeTypeDto
            {
                TypeId = dt.TypeId,
                Name = dt.TypeName,
                HaveFileAttachment = dt.HaveFile,

            })
            .ToListAsync();
    }

    public async Task<DemandeTypeDto?> GetDemandeTypeById(int id)
    {
        return await _db.DemandeTypes
            .Where(dt => dt.TypeId == id)
            .Select(dt => new DemandeTypeDto
            {
                TypeId = dt.TypeId,
                Name = dt.TypeName,
                HaveFileAttachment = dt.HaveFile,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<DemandeTypeDto?> CreateAsync(CreateDemandeTypeDto dto)
    {
        var demandeType = new DemandeType
        {
            TypeName = dto.Name,
            HaveFile = dto.HaveFileAttachment,
        };

        _db.DemandeTypes.Add(demandeType);
        await _db.SaveChangesAsync();

        return new DemandeTypeDto
        {
            TypeId = demandeType.TypeId,
            Name = demandeType.TypeName,
            HaveFileAttachment = demandeType.HaveFile,
        };
    }

    public async Task<DemandeTypeDto?> UpdateDemandeType(int id, UpdateDemandeTypeDto dto)
    {
        var demandeType = await _db.DemandeTypes.FindAsync(id);

        if (demandeType == null)
            return null;

        demandeType.TypeName = dto.Name;
        demandeType.HaveFile = dto.HaveFileAttachment;

        await _db.SaveChangesAsync();

        return new DemandeTypeDto
        {
            TypeId = demandeType.TypeId,
            Name = demandeType.TypeName,
            HaveFileAttachment = demandeType.HaveFile,
        };
    }
    public async Task<bool> DeleteDemandeType(int id)
    {
        var demandeType = await _db.DemandeTypes.FindAsync(id);

        if (demandeType == null)
            return false;

        _db.DemandeTypes.Remove(demandeType);
        await _db.SaveChangesAsync();

        return true;
    }

}