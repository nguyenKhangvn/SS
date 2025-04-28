using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class Chat : BaseEntity
    {
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        public ChatStatus Status { get; set; } = ChatStatus.ACTIVE;

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
    }
}
