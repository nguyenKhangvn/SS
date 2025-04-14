//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Ecommerce.Infrastructure.Data
//{
//    class EcommerceDbContextFactory : IDesignTimeDbContextFactory<EcommerceDbContext>
//    {
//        public EcommerceDbContext CreateDbContext(string[] args)
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<EcommerceDbContext>();
//            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../../Presentation/Ecommerce.API");
//            var configuration = new ConfigurationBuilder()
//                .SetBasePath(basePath)  // Chỉnh sửa đường dẫn
//                .AddJsonFile("appsettings.json")
//                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
//                .AddEnvironmentVariables()  // Ưu tiên biến môi trường từ Docker Compose
//                .Build();
//            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings_DefaultConnection");

//            if (string.IsNullOrWhiteSpace(connectionString))
//            {
//                connectionString = configuration.GetConnectionString("DefaultConnection");
//            }

//            optionsBuilder.UseNpgsql(connectionString);

//            return new EcommerceDbContext(optionsBuilder.Options);
//        }

//    }
//}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Ecommerce.Infrastructure.Data
{
    public class EcommerceDbContextFactory : IDesignTimeDbContextFactory<EcommerceDbContext>
    {
        public EcommerceDbContext CreateDbContext(string[] args)
        {
            // Sửa lại base path cho phù hợp với Docker container
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../../Ecommerce.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)  // Chỉnh sửa đường dẫn
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()  // Ưu tiên biến môi trường từ Docker Compose
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<EcommerceDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Could not find connection string. Check appsettings.json or environment variables.");
            }

            optionsBuilder.UseNpgsql(connectionString, opts =>
            {
                opts.EnableRetryOnFailure(3);  // Thêm retry khi kết nối
                opts.CommandTimeout(30);  // Tăng timeout
            });

            return new EcommerceDbContext(optionsBuilder.Options);
        }
    }
}