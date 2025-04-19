//using Asp.Versioning;


//using Microsoft.EntityFrameworkCore;

//namespace Ecommerce.API.Bootstraping;
//public static class ApplicationServiceExtensions
//{
//    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
//    {
//        builder.AddServiceDefaults();
//        builder.Services.AddOpenApi();

//        builder.Services.AddApiVersioning( //add versioning
//            opts => { 
//                opts.ReportApiVersions = true;
//                opts.ApiVersionReader = ApiVersionReader.Combine(
//                    new UrlSegmentApiVersionReader(), 
//                    new HeaderApiVersionReader("X-Version")
//                    );
//            }
//        );

//        builder.AddNpgsqlDbContext<EcommerceDbContext>("ecommerce-dbct", configureDbContextOptions: dbContextOptionsBuilder =>
//        {
//            dbContextOptionsBuilder.UseNpgsql(builder => builder.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName));
//        });
//        //builder.Services.AddDbContext<EcommerceDbContext>(options =>
//        //{
//        //    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//        //    // *** THÊM LOGGING Ở ĐÂY ***
//        //    Console.WriteLine($"--- Retrieved Connection String: {connectionString} ---"); // Hoặc dùng ILogger nếu đã có
//        //    if (string.IsNullOrWhiteSpace(connectionString))
//        //    {
//        //        Console.WriteLine("--- ERROR: Connection string 'DefaultConnection' is NULL or EMPTY! ---");
//        //    }
//        //    // *************************

//        //    options.UseNpgsql(connectionString,
//        //        npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName));
//        //});


//        return builder;
//    }
//}
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.API.Bootstraping;

public static class ApplicationServiceExtensions
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // 1. Cấu hình service defaults và OpenAPI
        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();

        // 2. Cấu hình API Versioning
        builder.Services.AddApiVersioning(opts =>
        {
            opts.ReportApiVersions = true;
            opts.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Version")
            );
        });


        // 3. Cấu hình DbContext (kết hợp Aspire + appsettings)
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // Log connection string để debug
        var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());

        //if (string.IsNullOrWhiteSpace(connectionString))
        //{
     
        //    // Fallback: Sử dụng Aspire nếu không có trong appsettings
        //    builder.AddNpgsqlDbContext<EcommerceDbContext>("ecommerce-dbct", configureDbContextOptions: options =>
        //    {
        //        options.UseNpgsql(npgsqlBuilder =>
        //        {
        //            npgsqlBuilder.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName);
        //            npgsqlBuilder.EnableRetryOnFailure(3);
        //        });
        //    });
        //}
        //else
        //{
            // Ưu tiên dùng connection string từ appsettings.json
            builder.Services.AddDbContext<EcommerceDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(3);
                });
            });
        //}
        builder.Services.AddHttpContextAccessor();

        return builder;
    }
}