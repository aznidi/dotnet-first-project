using FIRST.Models.Chat;

namespace FIRST.DTOs.Chat;

public class MessageDto
{
    public long Id { get; set; }
    public int ConversationId { get; set; }

    public int FromUserId { get; set; }
    public int ToUserId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public List<ReactionDto> Reactions { get; set; } = new();
}