using System.ComponentModel.DataAnnotations;

namespace FIRST.DTOs.Teachers;

public class CreateTeacherDto
{
    [Required, MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Cin { get; set; } = string.Empty;

    public DateOnly? DateNaissance { get; set; }
}
