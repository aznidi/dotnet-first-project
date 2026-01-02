using FIRST.Data;
using FIRST.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace FIRST.Services.Chat;

public class ChatService
{
    private readonly AppDbContext _db;

    public ChatService(AppDbContext db) => _db = db;

    public async Task<Conversation> GetOrCreateConversationAsync(int userA, int userB)
    {
        var u1 = Math.Min(userA, userB);
        var u2 = Math.Max(userA, userB);

        var conv = await _db.Conversations
            .FirstOrDefaultAsync(c => c.User1Id == u1 && c.User2Id == u2);

        if (conv != null) return conv;

        conv = new Conversation
        {
            User1Id = u1,
            User2Id = u2
        };

        _db.Conversations.Add(conv);
        await _db.SaveChangesAsync();
        return conv;
    }

    public async Task<Message> SavePrivateMessageAsync(int fromUserId, int toUserId, string content)
    {
        content = (content ?? "").Trim();
        if (content.Length == 0) throw new InvalidOperationException("Message is empty");

        var conv = await GetOrCreateConversationAsync(fromUserId, toUserId);

        var msg = new Message
        {
            ConversationId = conv.Id,
            SenderId = fromUserId,
            RecipientId = toUserId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        _db.Messages.Add(msg);

        conv.UpdatedAt = DateTime.UtcNow;
        conv.LastMessageAt = msg.SentAt;

        await _db.SaveChangesAsync();
        return msg;
    }
}
