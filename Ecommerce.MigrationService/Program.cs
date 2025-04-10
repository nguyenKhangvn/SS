using Ecommerce.Infrastructure.Data;
using Ecommerce.MigrationService;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

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



var host = builder.Build();
host.Run();
