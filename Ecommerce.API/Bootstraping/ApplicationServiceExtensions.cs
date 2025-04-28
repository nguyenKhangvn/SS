
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.API.Bootstraping;

public static class ApplicationServiceExtensions
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // 1. Cấu hình service defaults và OpenAPI
        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();
        builder.Services.AddAntiforgery();
        builder.Services.AddSignalR();
        // cấu hình authen gg
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookies";
            options.DefaultChallengeScheme = "Google";
        })
        .AddCookie("Cookies", options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.IsEssential = true;
        })
        .AddGoogle(options =>
        {
            options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
            options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            options.CallbackPath = "/signin-google";
        });
        // 2. Cấu hình API Versioning
        builder.Services.AddApiVersioning(opts =>
        {
            opts.ReportApiVersions = true;
            opts.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Version")
            );
        });
        // swagger
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Ecommerce API",
                Version = "v1",
                Description = "API tài liệu cho hệ thống Ecommerce",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "Your Name",
                    Email = "your@email.com",
                    Url = new Uri("https://your-website.com")
                }
            });
        });


        // 3. Cấu hình DbContext (kết hợp Aspire + appsettings)
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // Log connection string để debug
        var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());

        builder.Services.AddDbContext<EcommerceDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(EcommerceDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(3);
            });
        });

        builder.Services.AddHttpContextAccessor();

        return builder;
    }
}