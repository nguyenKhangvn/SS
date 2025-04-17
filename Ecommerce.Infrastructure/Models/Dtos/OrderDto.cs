using Ecommerce.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class OrderDto
    {
        public string OrderCode { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid? BillingAddressId { get; set; }
        public Guid? CouponId { get; set; }
        public Guid? PaymentId { get; set; }
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus Status { get; set; } = OrderStatus.PENDING;
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }

    public class UpdateOrderStatusDto
    {
        [EnumDataType(typeof(OrderStatus))]
        public string Status { get; set; }
    }

}
