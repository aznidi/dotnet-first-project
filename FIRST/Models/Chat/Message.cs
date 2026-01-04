using FIRST.Models;

namespace FIRST.Models.Chat;

public class Message
{
    public long Id { get; set; }

    public int ConversationId { get; set; }
    public Conversation? Conversation { get; set; }

    public int SenderId { get; set; }
    public User? Sender { get; set; }

    public int RecipientId { get; set; }
    public User? Recipient { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();


}
