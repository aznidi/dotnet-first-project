using System.ComponentModel.DataAnnotations;

namespace FIRST.Models;

public class DemandeType
{
    [Key]
    public int TypeId { get; set; }

    [Required, MaxLength(150)]
    public string TypeName { get; set; } = default!;

    public bool HaveFile { get; set; }

    // 1 -> N
    public ICollection<Demande> Demandes { get; set; } = new List<Demande>();
}
