
//using Ecommerce.API.Hubs;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddExtentionServices();
builder.AddApplicationServices();


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
}
app.UseHttpsRedirection();
//au 
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
//app.MapHub<ChatHub>("/chatHub");
//app.MapChatApi();
app.MapImageApi();
app.MapAuthApi();
app.MapProductStoreInventoryApi();
app.Run();

