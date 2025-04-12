//using Ecommerce.Infrastructure.Data;
//using Ecommerce.MigrationService;
//using Microsoft.EntityFrameworkCore;

//var builder = Host.CreateApplicationBuilder(args);

//builder.AddServiceDefaults();
//builder.Services.AddHostedService<Worker>();


//builder.AddNpgsqlDbContext<EcommerceDbContext>("ecommerce-dbct", configureDbContextOptions: dbContextOptionsBuilder =>
//{
//    dbContextOptionsBuilder.UseNpgsql(builder => builder.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName));
//});



//var host = builder.Build();
//host.Run();
using Ecommerce.Infrastructure.Data;
using Ecommerce.MigrationService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder(args);

// 1. Đọc config từ appsettings.json
var configuration = builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// 2. Đăng ký Worker
builder.Services.AddHostedService<Worker>();

// 3. Đăng ký DbContext với connection string từ appsettings
builder.Services.AddDbContext<EcommerceDbContext>(options =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName);
        npgsqlOptions.EnableRetryOnFailure(3);
    });
});

var host = builder.Build();
host.Run();