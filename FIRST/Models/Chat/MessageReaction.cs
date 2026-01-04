namespace FIRST.Models.Chat;

public class MessageReaction
{
    public long Id { get; set; }

    public long MessageId { get; set; }
    public Message? Message { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public string Type { get; set; } = "like";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
