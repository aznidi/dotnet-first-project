using FIRST.DTOs.Chat;
using FIRST.Models.Chat;

namespace FIRST.Repositories.Chat;

public interface IConversationRepository
{
    Task<Conversation> GetOrCreateAsync(int currentUserId, int otherUserId);

    /// returns null if conversation doesn't exist OR not owned by current user
    Task<List<MessageDto>?> GetMessagesAsync(int conversationId, int currentUserId, int skip, int take);
}