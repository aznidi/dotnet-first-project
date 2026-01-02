using FIRST.Data;
using FIRST.DTOs;
using FIRST.DTOs.Demandes;
using FIRST.Models;
using FIRST.Repositories.Students;
using Microsoft.EntityFrameworkCore;
 
namespace FIRST.Services;


public class DemandeService
{
    private readonly AppDbContext _db;

    public DemandeService(AppDbContext db)
    {
        _db = db;
    }


    public async Task<List<DemandeDto>> GetAllAsync()
    {
        return await _db.Demandes
            .Select(d => new DemandeDto
            {
                DemandeId = d.DemandeId,
                DemandeTypeId = d.DemandeTypeId,
                UserId = d.UserId,
                Motif = d.Motif,
                Status = (int)d.Status,
                DemandeDate = d.DemandeDate,
                DemandeType = new DemandeTypeLiteDto
                {
                    DemandeTypeId = d.DemandeType.TypeId,
                    Name = d.DemandeType.TypeName
                },
                User = new UserLiteDto
                {
                    UserId = d.User.Id,
                    FullName = d.User.FullName,
                    Email = d.User.Email
                }
            })
            .ToListAsync();
    }

    public async Task<DemandeDto?> GetDemandeById(int id)
    {
        var demande = await _db.Demandes.FindAsync(id);
        if (demande == null) return null;

        return new DemandeDto
        {
            DemandeId = demande.DemandeId,
            Motif = demande.Motif,
            Status = (int)demande.Status,
            DemandeDate = demande.DemandeDate,
            DemandeType = new DemandeTypeLiteDto
            {
                DemandeTypeId = demande.DemandeType.TypeId,
                Name = demande.DemandeType.TypeName
            },
            User = new UserLiteDto
            {
                UserId = demande.User.Id,
                FullName = demande.User.FullName,
                Email = demande.User.Email
            }
        };
    }   

    public async Task<DemandeDto> CreateAsync(CreateDemandeDto dto)
    {
        var demande = new Demande
        {
            DemandeTypeId = dto.DemandeTypeId,
            UserId = dto.UserId,
            Motif = dto.Motif,
        };

        _db.Demandes.Add(demande);
        await _db.SaveChangesAsync();

        return new DemandeDto
        {
            DemandeId = demande.DemandeId,
            DemandeTypeId = demande.DemandeTypeId,
            UserId = demande.UserId,
            Motif = demande.Motif,
            Status = (int)demande.Status,
            DemandeDate = demande.DemandeDate,
        };
    }

    public async Task<DemandeDto?> UpdateDemande(int id, UpdateDemandeDto dto)
    {
        var demande = await _db.Demandes.FindAsync(id);
        if (demande == null) return null;

        demande.DemandeTypeId = dto.DemandeTypeId;
        demande.UserId = dto.UserId;
        demande.Motif = dto.Motif;
        if (dto.Status.HasValue)
        {
            demande.Status = (Demande.DemandeStatus)dto.Status.Value;
        }
        

        await _db.SaveChangesAsync();

        return new DemandeDto
        {
            DemandeId = demande.DemandeId,
            Motif = demande.Motif,
            Status = (int)demande.Status,
            DemandeDate = demande.DemandeDate,
            DemandeType = new DemandeTypeLiteDto
            {
                DemandeTypeId = demande.DemandeType.TypeId,
                Name = demande.DemandeType.TypeName
            },
            User = new UserLiteDto
            {
                UserId = demande.User.Id,
                FullName = demande.User.FullName,
                Email = demande.User.Email
            }
        };
    }
    public async Task<bool> DeleteDemande(int id)
    {
        var demande = await _db.Demandes.FindAsync(id);
        if (demande == null) return false;

        _db.Demandes.Remove(demande);
        await _db.SaveChangesAsync();

        return true;
    }
}