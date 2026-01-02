using System.ComponentModel.DataAnnotations;

namespace FIRST.Models;

public class RefreshToken
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public User? User { get; set; }

    [Required]
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByTokenHash { get; set; }

    [MaxLength(200)]
    public string? UserAgent { get; set; }

    [MaxLength(50)]
    public string? IpAddress { get; set; }
}
