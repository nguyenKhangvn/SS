using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class ProductClickTracking : BaseEntity
    {
        public Guid ProductId { get; set; }
        public int ClickCount { get; set; }
    }
}
