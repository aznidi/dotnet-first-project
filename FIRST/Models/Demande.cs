using System.ComponentModel.DataAnnotations;

namespace FIRST.Models;

public class Demande
{
    [Key]
    public int DemandeId { get; set; }

    // FK -> DemandeType
    [Required]
    public int DemandeTypeId { get; set; }
    public DemandeType DemandeType { get; set; } = default!;

    // FK -> User
    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = default!;

    [Required, MaxLength(2000)]
    public string Motif { get; set; } = default!;

    [Required]
    public DemandeStatus Status { get; set; } = DemandeStatus.EnAttente;

    public enum DemandeStatus
    {
    EnAttente = 1,
    Approuvee = 2,
    Refusee = 3
   }


    public DateTime DemandeDate { get; set; } = DateTime.UtcNow;

    // Soft delete
    public DateTime? DeletedAt { get; set; }
}
