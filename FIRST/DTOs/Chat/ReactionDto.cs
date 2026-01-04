using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIRST.DTOs.Chat
{
    public class ReactionDto
    {
        public long Id { get; set; }

        public long MessageId { get; set; }

        public string? Type { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}