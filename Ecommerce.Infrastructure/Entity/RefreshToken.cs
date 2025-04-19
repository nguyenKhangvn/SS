using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Infrastructure.Entity
{
    public class RefreshToken : BaseEntity
    {
        [Required, MaxLength(500)]
        public string Token { get; set; } = string.Empty; // Lưu giá trị đã hash

        [Required]
        public DateTime Expires { get; set; } // Thời điểm hết hạn

        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow; // Ngày tạo

        public DateTime? Revoked { get; set; } // Thời điểm thu hồi

        [MaxLength(500)]
        public string? ReplacedByToken { get; set; } // Token thay thế (nếu có)

        [MaxLength(100)]
        public string? CreatedByIp { get; set; } // IP tạo token

        [MaxLength(200)]
        public string? DeviceInfo { get; set; } // Thông tin thiết bị

        // Foreign key
        [Required, ForeignKey("User")]
        public Guid UserId { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;

        // Helper properties
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}