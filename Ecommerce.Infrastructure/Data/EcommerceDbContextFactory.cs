using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Data
{
    class EcommerceDbContextFactory : IDesignTimeDbContextFactory<EcommerceDbContext>
    {
        public EcommerceDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EcommerceDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings_DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = "Host=localhost;Database=ecommerce;Username=postgres;Password=postgres";
            }

            optionsBuilder.UseNpgsql(connectionString);

            return new EcommerceDbContext(optionsBuilder.Options);
        }

    }
}
