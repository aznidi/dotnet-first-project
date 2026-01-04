using FIRST.Data;
using FIRST.DTOs.Chat;
using FIRST.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace FIRST.Repositories.Chat;

public class ConversationRepository : IConversationRepository
{
    private readonly AppDbContext _db;

    public ConversationRepository(AppDbContext db) => _db = db;

    public async Task<Conversation> GetOrCreateAsync(int currentUserId, int otherUserId)
    {
        var u1 = Math.Min(currentUserId, otherUserId);
        var u2 = Math.Max(currentUserId, otherUserId);

        var conv = await _db.Conversations
            .FirstOrDefaultAsync(c => c.User1Id == u1 && c.User2Id == u2);

        if (conv != null) return conv;

        conv = new Conversation
        {
            User1Id = u1,
            User2Id = u2,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Conversations.Add(conv);
        await _db.SaveChangesAsync();

        return conv;
    }

    public async Task<List<MessageDto>?> GetMessagesAsync(int conversationId, int currentUserId, int skip, int take)
    {
        // Security: user must be participant in this conversation
        var allowed = await _db.Conversations
            .AsNoTracking()
            .AnyAsync(c =>
                c.Id == conversationId &&
                (c.User1Id == currentUserId || c.User2Id == currentUserId)
            );

        if (!allowed) return null;

        var items = await _db.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .Include(m => m.Reactions)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                FromUserId = m.SenderId,
                ToUserId = m.RecipientId,
                Content = m.Content,
                SentAt = m.SentAt,
                ReadAt = m.ReadAt,
                Reactions = m.Reactions.Select(r => new ReactionDto
                    {
                        Id = r.Id,
                        MessageId = r.MessageId,
                        Type = r.Type,
                        UserId = r.UserId,
                        CreatedAt = r.CreatedAt
                    }
                    ).ToList()
            })
            .ToListAsync();

        // return chronological order for chat display
        return items.OrderBy(x => x.SentAt).ToList();
    }
}
