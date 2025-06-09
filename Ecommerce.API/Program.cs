using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Cache
builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = "Cookies";
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
})
.AddCookie("Cookies")
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/signin-google";
    options.SaveTokens = true;
});


// Các dịch vụ mở rộng ap
builder.Services.AddExtentionServices();
builder.AddApplicationServices();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5173", 
                            "http://localhost:3000", 
                            "http://14.225.253.218:3000", 
                            "https://yensao.hoanggiaquynhon.io.vn")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseStaticFiles();

app.UseCors("AllowSpecificOrigin");

app.UseSession();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map các Hubs
app.MapHub<ChatHub>("/chatHub");
app.MapHub<ReviewHub>("/reviewHub");

// Map các API
app.MapDefaultEndpoints();

app.MapProductApi();
app.MapCategoryApi();
app.MapStoreLocationApi();
app.MapPostApi();
app.MapUserApi();
app.MapManufacturerApi();
app.MapAddressApi();
app.MapOrderApi();
app.MapCouponApi();
app.MapOrderItemApi();
app.MapPaymentApi();

app.MapChatApi();
app.MapImageApi();
app.MapAuthApi();
app.MapProductStoreInventoryApi();
app.MapReportApi();
app.MapReviewApi();

app.Run();
