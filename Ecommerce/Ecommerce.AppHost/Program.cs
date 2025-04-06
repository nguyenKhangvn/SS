var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
            .WithImageTag("latest")
            .WithVolume("ecommerce-db", "/var/lib/postgresql/data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithPgWeb();

var ecommerceDb = postgres.AddDatabase("ecommerce-db", "ecommerce");

var migrationService = builder.AddProject<Projects.Ecommerce_MigrationService>("ecommerce-migrationservice")
                        .WithReference(ecommerceDb)
                        .WaitFor(ecommerceDb);


builder.AddProject<Projects.Ecommerce_API>("ecommerce-api")
    .WithReference(ecommerceDb)
    .WaitFor(postgres)
    .WaitForCompletion(migrationService); ;
;


builder.Build().Run();
