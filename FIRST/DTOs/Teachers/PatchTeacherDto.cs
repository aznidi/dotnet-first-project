using System.ComponentModel.DataAnnotations;

namespace FIRST.DTOs.Teachers;

public class PatchTeacherDto
{
    [MaxLength(100)]
    public string? Nom { get; set; }

    [MaxLength(100)]
    public string? Prenom { get; set; }

    [MaxLength(20)]
    public string? Cin { get; set; }

    public DateOnly? DateNaissance { get; set; }
}
