using Asp.Versioning;


using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Bootstraping;
public static class ApplicationServiceExtensions
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();

        builder.Services.AddApiVersioning( //add versioning
            opts => { 
                opts.ReportApiVersions = true;
                opts.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(), 
                    new HeaderApiVersionReader("X-Version")
                    );
            }
        );

        //builder.AddNpgsqlDbContext<EcommerceDbContext>("ecommerce-db", configureDbContextOptions: dbContextOptionsBuilder =>
        //{
        //    dbContextOptionsBuilder.UseNpgsql(builder => builder.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName));
        //});
        builder.Services.AddDbContext<EcommerceDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // *** THÊM LOGGING Ở ĐÂY ***
            Console.WriteLine($"--- Retrieved Connection String: {connectionString} ---"); // Hoặc dùng ILogger nếu đã có
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine("--- ERROR: Connection string 'DefaultConnection' is NULL or EMPTY! ---");
            }
            // *************************

            options.UseNpgsql(connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName));
        });


        return builder;
    }
}
