using System.ComponentModel.DataAnnotations;

namespace FIRST.DTOs.Classes.Request
{
    public class UpdateClassDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
        public int Capacity { get; set; }
    }
}
