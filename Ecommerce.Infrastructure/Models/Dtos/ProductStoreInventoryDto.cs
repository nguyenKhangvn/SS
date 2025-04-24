using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class ProductStoreInventoryDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid StoreLocationId { get; set; }
        public int Quantity { get; set; }

        public string? ProductName { get; set; }
        public string? StoreName { get; set; }
    }

    public class AddOrUpdateProductStoreInventoryDto
    {
        public Guid ProductId { get; set; }
        public Guid StoreLocationId { get; set; }
        public int Quantity { get; set; }
    }
}
