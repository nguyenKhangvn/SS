using Ecommerce.API.Apis;
using Ecommerce.API.Bootstraping;
using Ecommerce.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.Services.AddScoped<IProductRepository, ProductService>();
var app = builder.Build();

app.MapDefaultEndpoints();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.MapProductApi();
app.Run();

