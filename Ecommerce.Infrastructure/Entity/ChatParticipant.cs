using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class ChatParticipant : BaseEntity // Include BaseEntity if you need CreatedAt/UpdatedAt for participation
    {
        // Foreign Guid
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("ChatId")]
        public virtual Chat? Chat { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
