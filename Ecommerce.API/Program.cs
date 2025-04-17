using Ecommerce.API.Apis;
using Ecommerce.API.Bootstraping;
using Ecommerce.API.Extention;
//using Ecommerce.API.Hubs;
using Ecommerce.API.Services;
using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Mapping;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery();
builder.AddApplicationServices();
builder.Services.AddExtentionServices();
builder.Services.AddSignalR();
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultEndpoints();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
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
app.Run();

