using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FIRST.DTOs.Demandes
{
    public class CreateDemandeDto
    {
        [Required]
        public int DemandeTypeId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? Status { get; set; } 


        [Required, MaxLength(2000)]
        public string Motif { get; set; } = string.Empty;

        
    }
}