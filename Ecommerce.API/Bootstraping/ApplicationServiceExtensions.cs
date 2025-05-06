using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.API.Bootstraping;

public static class ApplicationServiceExtensions
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // 1. Service defaults and OpenAPI
        builder.AddServiceDefaults();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();

        builder.Services.AddOpenApi();
        builder.Services.AddAntiforgery();
        builder.Services.AddSignalR();

        // 2. Authentication (Google)
        //builder.Services.AddAuthentication(options =>
        //{
        //    options.DefaultScheme = "Cookies";
        //    options.DefaultChallengeScheme = "Google";
        //})
        //.AddCookie("Cookies", options =>
        //{
        //    options.Cookie.SameSite = SameSiteMode.None;
        //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        //    options.Cookie.IsEssential = true;
        //});


        // 3. Redirect handling for APIs
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            };
        });

        // 4. API Versioning
        builder.Services.AddApiVersioning(opts =>
        {
            opts.ReportApiVersions = true;
            opts.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Version")
            );
        });

        // 5. Swagger
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Ecommerce API",
                Version = "v1",
                Description = "API documentation for the Ecommerce system",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "Your Name",
                    Email = "your@email.com",
                    Url = new Uri("https://your-website.com")
                }
            });
        });

        // 6. Database
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

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
