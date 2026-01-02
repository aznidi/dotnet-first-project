using System.Security.Claims;
using FIRST.DTOs.Contacts;
using FIRST.Repositories.Contacts;

namespace FIRST.Services.Contacts;

public class ContactService
{
    private readonly IContactRepository _repo;

    public ContactService(IContactRepository repo) => _repo = repo;

    public Task<List<ContactDto>> GetContactsAsync(int currentUserId, string? q, int page, int perPage)
        => _repo.GetContactsAsync(currentUserId, q, page, perPage);
}
