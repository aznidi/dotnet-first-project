using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FIRST.DTOs
{
    public class CreateStudentDto
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