using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class AddressDto
    {
        public Guid? Id { get; set; } // Cho update
        public Guid UserId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = "Vietnam";
        public string? PostalCode { get; set; }
        public bool IsDefaultShipping { get; set; }
        public bool IsDefaultBilling { get; set; }
        //public bool IsDeleted { get; set; } = false;
    }
}
