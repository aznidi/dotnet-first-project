using FIRST.Services.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FIRST.Hubs;

[Authorize]
public class AppHub : Hub
{

    private readonly ChatService _chat;

    public AppHub(ChatService chat) => _chat = chat;

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? Context.User?.FindFirstValue("sub");

        await Clients.Caller.SendAsync("Connected", new { userId });

        await base.OnConnectedAsync();
    }

    // Message privé: 1 -> 1
     public async Task SendPrivate(string toUserId, string message)
    {
        var fromStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? Context.User?.FindFirstValue("sub");

        if (!int.TryParse(fromStr, out var fromId))
            throw new InvalidOperationException("Invalid token: missing user id");

        if (!int.TryParse(toUserId, out var toId))
            throw new InvalidOperationException("Invalid recipient id");

        var saved = await _chat.SavePrivateMessageAsync(fromId, toId, message);

        var payload = new
        {
            id = saved.Id,
            conversationId = saved.ConversationId,
            fromUserId = saved.SenderId,
            toUserId = saved.RecipientId,
            message = saved.Content,
            sentAt = saved.SentAt
        };

        await Clients.User(toId.ToString()).SendAsync("PrivateMessage", payload);
        await Clients.User(fromId.ToString()).SendAsync("PrivateMessage", payload);

    }
}
