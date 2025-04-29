var builder = WebApplication.CreateBuilder(args);
// Add SignalR
builder.Services.AddSignalR();
//cache
builder.Services.AddMemoryCache();

builder.Services.AddExtentionServices();
builder.AddApplicationServices();

//add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseStaticFiles();
app.MapDefaultEndpoints();

app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
}
app.UseHttpsRedirection();
//au
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapProductApi();
app.MapCategoryApi();
app.MapStoreLocationApi();
app.MapPostApi();
app.MapUserApi();
app.MapManufacturerApi();
app.MapAddressApi();
app.MapOrderApi();
app.MapCouponAPi();
app.MapOrderItemApi();
app.MapPaymentApi();
app.MapHub<ChatHub>("/chatHub");
app.MapChatApi();
app.MapImageApi();
app.MapAuthApi();
app.MapProductStoreInventoryApi();
app.MapReportApi();
app.Run();