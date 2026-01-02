using System.ComponentModel.DataAnnotations;

namespace FIRST.DTOs.Subjects;

public class PatchSubjectDto
{
    [MaxLength(150)]
    public string? Name { get; set; }

    [MaxLength(20)]
    public string? Code { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool? IsActive { get; set; }
}
