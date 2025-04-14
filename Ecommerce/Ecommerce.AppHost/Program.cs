var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
            .WithImageTag("latest")
            .WithVolume("ecommerce-dbct", "/var/lib/postgresql/data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithPgWeb();

var ecommerceDb = postgres.AddDatabase("ecommerce-dbct", "ecommerce");

var migrationService = builder.AddProject<Projects.Ecommerce_MigrationService>("ecommerce-migrationservice")
                         //.WithReference(ecommerceDb)
                        .WithEnvironment("ConnectionStrings__DefaultConnection", "Host=interchange.proxy.rlwy.net;Port=36251;Database=railway;Username=postgres;Password=RSXCNtCPkscFmrKSYCDRbjAvKtNsikAZ")
                        .WaitFor(ecommerceDb);


builder.AddProject<Projects.Ecommerce_API>("ecommerce-api")
     //.WithReference(ecommerceDb)
     //.WaitFor(postgres)
     .WithEnvironment("ConnectionStrings__DefaultConnection", "Host=interchange.proxy.rlwy.net;Port=36251;Database=railway;Username=postgres;Password=RSXCNtCPkscFmrKSYCDRbjAvKtNsikAZ")
     .WaitForCompletion(migrationService);
;


builder.Build().Run();
