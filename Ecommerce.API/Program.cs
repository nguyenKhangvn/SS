using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cache
builder.Services.AddMemoryCache();

// ✅ Cấu hình xác thực JWT + Google chỉ trong MỘT lệnh AddAuthentication()
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


// Các dịch vụ mở rộng
builder.Services.AddExtentionServices();
builder.AddApplicationServices();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Cấu hình CORS chuẩn, tránh lỗi AllowCredentials + AllowAnyOrigin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // ⚠️ KHÔNG dùng AllowAnyOrigin nếu đã AllowCredentials
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseStaticFiles();

// ✅ CORS phải đặt TRƯỚC Authentication/Authorization
app.UseCors("AllowSpecificOrigin");

app.UseSession();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // ✅ sau CORS
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
