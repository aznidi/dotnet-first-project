using FIRST.DTOs.Contacts;

namespace FIRST.Repositories.Contacts;

public interface IContactRepository
{
    Task<List<ContactDto>> GetContactsAsync(int currentUserId, string? q, int page, int perPage);
}
