using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Ecommerce.Infrastructure.Entity
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        [EmailAddress] // Basic email validation
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)] // Length depends on hashing algorithm
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; } // Nullable

        [Required]
        [MaxLength(30)]
        public RoleStatus Role { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(100)]
        public string? ResetToken { get; set; } // Nullable

        public DateTime? ResetTokenExpiry { get; set; } // Nullable

        // Navigation Properties
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
        public virtual ICollection<ChatParticipant> ChatParticipants { get; set; } = new List<ChatParticipant>();

    }
}
