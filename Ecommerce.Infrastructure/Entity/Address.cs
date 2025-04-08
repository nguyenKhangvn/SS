using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class Address : BaseEntity
    {

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        [MaxLength(100)]
        public string ReceiverName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string StreetAddress { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Ward { get; set; } // Phường/Xã

        [MaxLength(100)]
        public string? District { get; set; } // Quận/Huyện

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty; // Tỉnh/Thành phố

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = "Vietnam"; // Default?

        [MaxLength(20)]
        public string? PostalCode { get; set; } // Nullable

        public bool IsDefaultShipping { get; set; } = false;
        public bool IsDefaultBilling { get; set; } = false;
        //public bool IsDeleted { get; set; } = false;
    }
}
