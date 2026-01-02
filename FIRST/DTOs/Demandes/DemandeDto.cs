namespace FIRST.DTOs.Demandes;

public class DemandeDto
{
    public int DemandeId { get; set; }
    public string Motif { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime DemandeDate { get; set; }

    public int DemandeTypeId { get; set; }
    public int UserId { get; set; }

    public DemandeTypeLiteDto? DemandeType { get; set; }
    public UserLiteDto? User { get; set; }
}
