using Ecommerce.Infrastructure.Data;
using Ecommerce.MigrationService;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.AddNpgsqlDbContext<EcommerceDbContext>("ecommerce-db", configureDbContextOptions: dbContextOptionsBuilder =>
{
    dbContextOptionsBuilder.UseNpgsql(builder => builder.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName));
}
);


var host = builder.Build();
host.Run();
