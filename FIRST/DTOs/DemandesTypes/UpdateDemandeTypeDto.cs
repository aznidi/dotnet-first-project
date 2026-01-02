using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace FIRST.DTOs.DemandesTypes
{
    public class UpdateDemandeTypeDto
    {

        [Required]
        public int TypeId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool HaveFileAttachment { get; set; }= false;

    }
}