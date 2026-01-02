using System.ComponentModel.DataAnnotations;

namespace FIRST.DTOs
{
    public class UpdateStudentDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateOnly DateNaissance { get; set; }

        [Required, MaxLength(20)]
        public string Cin { get; set; } = string.Empty;
    }
}
