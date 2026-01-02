using System.ComponentModel.DataAnnotations;

namespace FIRST.DTOs
{
    public class PatchStudentDto
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        public DateOnly? DateNaissance { get; set; }

        [MaxLength(20)]
        public string? Cin { get; set; }
    }
}
