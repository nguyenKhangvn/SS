using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Apis
{
    public static class AddressApi
    {
        public static IEndpointRouteBuilder MapAddressApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);
            //[GET] http://localhost:5000/api/v1/ecommerce/address/
            v1.MapGet("/address", async ([FromServices] IAddressService service) =>
            {
                return Results.Ok(await service.GetAllAsync());
            });
            //[GET] http://localhost:5000/api/v1/ecommerce/address/
            v1.MapGet("/address/{id:guid}", async (Guid id, [FromServices] IAddressService service) =>
            {
                var result = await service.GetByIdAsync(id);
                return result == null ? Results.NotFound() : Results.Ok(result);
            });
            //[GET] http://localhost:5000/api/v1/ecommerce/address/
            v1.MapPost("/address", async ([FromBody] AddressDto dto, [FromServices] IAddressService service) =>
            {
                var created = await service.CreateAsync(dto);
                return Results.Created($"/{created.Id}", created);
            });
            //[PUT] http://localhost:5000/api/v1/ecommerce/address/
            v1.MapPut("/address/{id:guid}", async ([FromBody] AddressDto dto, [FromServices] IAddressService service) =>
            {
                var updated = await service.UpdateAsync(dto);
                return updated == null ? Results.NotFound() : Results.Ok(updated);
            });
            //[GET] http://localhost:5000/api/v1/ecommerce/address/
            v1.MapDelete("/address/{id:guid}", async (Guid id, [FromServices] IAddressService service) =>
            {
                var deleted = await service.DeleteAsync(id);
                return deleted ? Results.Ok() : Results.NotFound();
            });
            return builder;
        }
    }
}
