using FIRST.Services.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FIRST.Hubs;

[Authorize]
public class AppHub : Hub
{

    private readonly ChatService _chat;
    private readonly IPresenceTracker _presenceTracker;

    public AppHub(ChatService chat, IPresenceTracker presenceTracker)
    {
        _chat = chat;
        _presenceTracker = presenceTracker;
    }

    public override async Task OnConnectedAsync()
    {
        var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? Context.User?.FindFirstValue("sub");

        if(int.TryParse(userIdStr, out var userId ))
            _presenceTracker.Connected(userId, Context.ConnectionId);

        await Clients.Caller.SendAsync("Connected", new { userId });

    
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? Context.User?.FindFirstValue("sub");

        if (int.TryParse(userIdStr, out var userId))
            _presenceTracker.Disconnected(userId, Context.ConnectionId);


        await Clients.Caller.SendAsync("Disconnected", new { userId });

        await base.OnDisconnectedAsync(exception);
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

        if (_presenceTracker.IsOnline(toId))
                await _chat.MarkDeliveredAsync(saved.Id);
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

        if (_presenceTracker.IsOnline(toId))
        await Clients.User(fromId.ToString()).SendAsync("MessageDelivered", new { messageId = saved.Id, deliveredAt = DateTime.UtcNow });

    }

    public async Task MarkAsRead(int conversationId, string otherUserId)
    {
        var meStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Context.User?.FindFirstValue("sub");

        if (!int.TryParse(meStr, out var me)) throw new InvalidOperationException("Invalid user");
        if (!int.TryParse(otherUserId, out var other)) throw new InvalidOperationException("Invalid other user");

        var count = await _chat.MarkConversationReadAsync(conversationId, me);

        await Clients.User(other.ToString()).SendAsync("MessagesRead", new
        {
            conversationId,
            readerId = me,
            readAt = DateTime.UtcNow,
            count
        });
    }

    public async Task ReactToMessage(long messageId, string type)
    {
        var meStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Context.User?.FindFirstValue("sub");

        if (!int.TryParse(meStr, out var me)) throw new InvalidOperationException("Invalid user");

        var reaction = await _chat.AddReactionAsync(messageId, me, type);

        await Clients.All.SendAsync("MessageReaction", new
        {
            messageId,
            userId = me,
            type,
            createdAt = reaction.CreatedAt
        });
    }
}
