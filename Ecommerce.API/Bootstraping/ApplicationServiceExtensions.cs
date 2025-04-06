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

        builder.AddNpgsqlDbContext<EcommerceDbContext>("ecommerce-db", configureDbContextOptions: dbContextOptionsBuilder =>
        {
            dbContextOptionsBuilder.UseNpgsql(builder => builder.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName));
        });

     

        return builder;
    }
}
