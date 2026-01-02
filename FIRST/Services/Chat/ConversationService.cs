using FIRST.DTOs.Chat;
using FIRST.Repositories.Chat;

namespace FIRST.Services.Chat;

public class ConversationService
{
    private readonly IConversationRepository _repo;

    public ConversationService(IConversationRepository repo) => _repo = repo;

    public async Task<ConversationReadyDto> GetOrCreateWithUserAsync(int currentUserId, int otherUserId)
    {
        var conv = await _repo.GetOrCreateAsync(currentUserId, otherUserId);

        return new ConversationReadyDto
        {
            ConversationId = conv.Id,
            OtherUserId = otherUserId
        };
    }

    public Task<List<MessageDto>?> GetMessagesAsync(int conversationId, int currentUserId, int skip, int take)
        => _repo.GetMessagesAsync(conversationId, currentUserId, skip, take);
}
