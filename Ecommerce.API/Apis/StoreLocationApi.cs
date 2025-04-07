using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Apis
{
    public static class StoreLocationApi
    {
        public static IEndpointRouteBuilder MapStoreLocationApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            v1.MapPost("/store-location", (IStoreLocationService storeLocationService, StoreLocationDto storeLocation) => storeLocationService.AddAsync(storeLocation));
            v1.MapGet("/store-location/{storeLocationId:guid}", async (IStoreLocationService service, Guid storeLocationId, [FromQuery] string? includeProperties = null) =>
            {
                var storeLocation = await service.GetByIdAsync(storeLocationId, includeProperties);
                return storeLocation is not null ? Results.Ok(storeLocation) : Results.NotFound();
            });
            v1.MapGet("/store-location", async (IStoreLocationService service, [FromQuery] string? includeProperties = null) =>
            {
                var storeLocations = await service.GetStoreLocations();
                return Results.Ok(storeLocations);
            });
            v1.MapPut("/store-location/{storeLocationId:guid}", async (IStoreLocationService service, Guid storeLocationId, StoreLocationDto dto) =>
            {
                if (storeLocationId != dto.Id)
                {
                    return Results.BadRequest("ID trong URL không khớp với body");
                }

                var updated = await service.UpdateAsync(dto);
                return Results.Ok(updated);
            });
            v1.MapDelete("/store-location/{storeLocationId:guid}", async (IStoreLocationService service, Guid storeLocationId) =>
            {
                var success = await service.DeleteAsync(storeLocationId);
                return success ? Results.Ok() : Results.NotFound();
            });

            return builder;
        }
    }
}
