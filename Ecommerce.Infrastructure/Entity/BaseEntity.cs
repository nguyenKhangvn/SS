﻿using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Infrastructure.Entity
{
    public enum OrderStatus { PENDING, PROCESSING, SHIPPED, DELIVERED, CANCELED, RETURNED, COMPLETED }
    public enum PaymentStatus { PENDING, COMPLETED, FAILED, REFUNDED }
    public enum ShipmentStatus { PENDING, IN_TRANSIT, DELIVERED, FAILED }
    public enum DiscountType { PERCENTAGE, FIXED_AMOUNT }
    public enum ChatStatus { ACTIVE, CLOSED }
    public enum RoleStatus { ADMIN, STAFF, CUSTOMER }
    public abstract class BaseEntity
    {
        [Key] 
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
