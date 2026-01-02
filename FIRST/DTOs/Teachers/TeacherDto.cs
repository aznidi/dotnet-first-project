namespace FIRST.DTOs.Teachers;

public class TeacherDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Cin { get; set; } = string.Empty;
    public DateOnly? DateNaissance { get; set; }
}
