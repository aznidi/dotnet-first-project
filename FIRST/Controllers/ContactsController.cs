using System.Security.Claims;
using FIRST.Services.Contacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIRST.Controllers;

[Authorize]
[Route("api/[controller]")]
public class ContactsController : BaseApiController
{
    private readonly ContactService _service;

    public ContactsController(ContactService service) => _service = service;

    // GET /api/contacts?q=salah&page=1&perPage=20
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int perPage = 20)
    {
        if (page < 1) page = 1;
        if (perPage < 1) perPage = 20;
        if (perPage > 100) perPage = 100;

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(userIdStr, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid Token");

        var contacts = await _service.GetContactsAsync(currentUserId, q, page, perPage);
        return ApiOk(contacts, "Contacts retrieved");
    }
}
