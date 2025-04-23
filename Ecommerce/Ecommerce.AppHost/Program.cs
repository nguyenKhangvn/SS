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
     .WithEnvironment("VnPay__TmnCode", "4RDRDK85")
     .WithEnvironment("VnPay__HashSecret", "YOUQN1ZD393OLJ6C4GAQKSD2UUS2MRPQ")
     .WithEnvironment("VnPay__BaseUrl", "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html")
     .WithEnvironment("VnPay__Version", "2.1.0")
     .WithEnvironment("VnPay__Command", "pay")
     .WithEnvironment("VnPay__CurrCode", "VND")
     .WithEnvironment("VnPay__Locale", "vn")
     .WithEnvironment("VnPay__ReturnUrl", "http://localhost:5173/orders/payment/response")
     .WithEnvironment("TimeZoneId", "SE Asia Standard Time")
     .WithEnvironment("ConnectionStrings__DefaultConnection", "Host=interchange.proxy.rlwy.net;Port=36251;Database=railway;Username=postgres;Password=RSXCNtCPkscFmrKSYCDRbjAvKtNsikAZ")
     .WaitForCompletion(migrationService);
;



builder.Build().Run();
