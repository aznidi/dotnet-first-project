using FIRST.Data;
using FIRST.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace FIRST.Services.Chat;

public class ChatService
{
    private readonly AppDbContext _db;

    public ChatService(AppDbContext db) => _db = db;

    private static readonly HashSet<string> _emogies = new ()
    {
         "👍", "❤️", "😂", "😮", "😢", "😡"
    };
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

    public async Task<bool> MarkDeliveredAsync(long messageId)
    {
        var msg = await _db.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
        if (msg == null) return false;

        if (msg.DeliveredAt == null)
        {
            msg.DeliveredAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return true;
    }

    public async Task<int> MarkConversationReadAsync(int conversationId, int readerId)
    {
        var now = DateTime.UtcNow;

        var unread = await _db.Messages
            .Where(m => m.ConversationId == conversationId
                        && m.RecipientId == readerId
                        && m.ReadAt == null)
            .ToListAsync();

        foreach (var m in unread)
            m.ReadAt = now;

        await _db.SaveChangesAsync();
        return unread.Count;
    }

    public async Task<MessageReaction> AddReactionAsync(long messageId, int userId, string type)
    {
        type = (type ?? "").Trim();

        if (!_emogies.Contains(type))
            throw new InvalidOperationException("Invalid reaction type. Allowed: 👍 ❤️ 😂 😮 😢 😡");

        var msgExists = await _db.Messages.AnyAsync(m => m.Id == messageId);
        if (!msgExists)
            throw new InvalidOperationException("Message not found.");

        var existing = await _db.MessageReactions
            .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId && r.Type == type);

        if (existing != null)
        {
            _db.MessageReactions.Remove(existing);
            await _db.SaveChangesAsync();
            return existing;
        }

        var reaction = new MessageReaction
        {
            MessageId = messageId,
            UserId = userId,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };

        _db.MessageReactions.Add(reaction);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var raced = await _db.MessageReactions
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId && r.Type == type);

            if (raced != null) return raced;

            throw;
        }

        return reaction;
    }
}
