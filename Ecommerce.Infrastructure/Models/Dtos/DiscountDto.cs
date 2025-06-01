using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class DiscountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDiscountDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class UpdateDiscountDto : CreateDiscountDto
    {
        public bool IsActive { get; set; }
    }

}
