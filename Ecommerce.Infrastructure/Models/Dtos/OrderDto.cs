using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid? BillingAddressId { get; set; }
        public Guid? CouponId { get; set; }
        public Guid? PaymentId { get; set; }
        public string? Notes { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new();
    }

}
