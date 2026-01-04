using System.ComponentModel.DataAnnotations;

namespace FIRST.Models.Classes
{
    public class Class
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public int Capacity { get; set; }
    }
}