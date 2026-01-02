using FIRST.Data;
using FIRST.DTOs.Contacts;
using Microsoft.EntityFrameworkCore;

namespace FIRST.Repositories.Contacts;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _db;

    public ContactRepository(AppDbContext db) => _db = db;

    public async Task<List<ContactDto>> GetContactsAsync(int currentUserId, string? q, int page, int perPage)
    {
        var query = _db.Users
            .AsNoTracking()
            .Where(u => u.IsActive && u.Id != currentUserId);

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim().ToLower();
            query = query.Where(u =>
                (u.FullName ?? "").ToLower().Contains(q) ||
                u.Email.ToLower().Contains(q)
            );
        }

        return await query
            .OrderBy(u => u.FullName)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(u => new ContactDto
            {
                Id = u.Id,
                FullName = u.FullName ?? "",
                Email = u.Email
            })
            .ToListAsync();
    }
}
