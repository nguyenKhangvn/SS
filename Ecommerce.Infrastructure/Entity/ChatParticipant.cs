using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class ChatParticipant : BaseEntity
    {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ChatId")]
        public virtual Chat? Chat { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
