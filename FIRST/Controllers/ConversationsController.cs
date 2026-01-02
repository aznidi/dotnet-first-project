using System.Security.Claims;
using FIRST.Services.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIRST.Controllers;

[Authorize]
[Route("api/[controller]")]
public class ConversationsController : BaseApiController
{
    private readonly ConversationService _service;

    public ConversationsController(ConversationService service) => _service = service;

    // A) Get/Create conversation with user
    // GET /api/conversations/with/7
    [HttpGet("with/{otherUserId:int:min(1)}")]
    public async Task<IActionResult> GetOrCreateWithUser(int otherUserId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(userIdStr, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid token: missing user id");

        if (otherUserId == currentUserId)
            return ApiBadRequest("You cannot create a conversation with yourself");

        var data = await _service.GetOrCreateWithUserAsync(currentUserId, otherUserId);
        return ApiOk(data, "Conversation ready");
    }

    // B) Get messages history
    // GET /api/conversations/12/messages?skip=0&take=50
    [HttpGet("{conversationId:int:min(1)}/messages")]
    public async Task<IActionResult> GetMessages(int conversationId, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(userIdStr, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid token: missing user id");

        if (skip < 0) skip = 0;
        if (take < 1) take = 50;
        if (take > 100) take = 100;

        var messages = await _service.GetMessagesAsync(conversationId, currentUserId, skip, take);

        if (messages == null)
            return ApiNotFound("Conversation not found");

        return ApiOk(messages, "Messages retrieved");
    }
}
