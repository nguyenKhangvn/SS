using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class Message : BaseEntity
    {
        // Foreign Keys
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty; // Consider max length or TEXT type

        public DateTime SentAt { get; set; } // Can use BaseEntity.CreatedAt or have specific SentAt

        // Navigation Properties
        [ForeignKey("ChatId")]
        public virtual Chat? Chat { get; set; }

        [ForeignKey("SenderId")]
        public virtual User? Sender { get; set; }
    }
}
