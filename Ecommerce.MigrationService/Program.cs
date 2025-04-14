using Ecommerce.Infrastructure.Data;
using Ecommerce.MigrationService;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();


//builder.AddNpgsqlDbContext<EcommerceDbContext>("ecommerce-dbct", configureDbContextOptions: dbContextOptionsBuilder =>
//{
//    dbContextOptionsBuilder.UseNpgsql(builder => builder.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName));
//});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EcommerceDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName);
        npgsqlOptions.EnableRetryOnFailure(3);
    });
});


var host = builder.Build();
host.Run();
