using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var postgres = builder.AddPostgres("postgres")
            .WithImageTag("latest")
            .WithVolume("ecommerce-dbct", "/var/lib/postgresql/data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithPgWeb();

var ecommerceDb = postgres.AddDatabase("ecommerce-dbct", "ecommerce");

var migrationService = builder.AddProject<Projects.Ecommerce_MigrationService>("ecommerce-migrationservice")
                        //.WithReference(ecommerceDb)
                        .WithEnvironment("ConnectionStrings__DefaultConnection", configuration["ConnectionStrings:DefaultConnection"])
                        .WaitFor(ecommerceDb);

builder.AddProject<Projects.Ecommerce_API>("ecommerce-api")
    //.WithReference(ecommerceDb)
    //.WaitFor(postgres)
    .WithEnvironment("VnPay__TmnCode", configuration["VnPay:TmnCode"])
    .WithEnvironment("VnPay__HashSecret", configuration["VnPay:HashSecret"])
    .WithEnvironment("VnPay__BaseUrl", configuration["VnPay:BaseUrl"])
    .WithEnvironment("VnPay__Version", configuration["VnPay:Version"])
    .WithEnvironment("VnPay__Command", configuration["VnPay:Command"])
    .WithEnvironment("VnPay__CurrCode", configuration["VnPay:CurrCode"])
    .WithEnvironment("VnPay__Locale", configuration["VnPay:Locale"])
    .WithEnvironment("VnPay__ReturnUrl", configuration["VnPay:ReturnUrl"])
    .WithEnvironment("VnPay__TimeZoneId", configuration["VnPay:TimeZoneId"])
    .WithEnvironment("ConnectionStrings__DefaultConnection", configuration["ConnectionStrings:DefaultConnection"])
    .WaitForCompletion(migrationService);
;

builder.Build().Run();
