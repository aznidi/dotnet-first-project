using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace FIRST.DTOs.Demandes
{
    public class UpdateDemandeDto
    {
        public int DemandeId { get; set; }

        public int DemandeTypeId { get; set; }

        public int UserId { get; set; }

        public int? Status { get; set; }
        public string Motif { get; set; } = string.Empty;

    }
}