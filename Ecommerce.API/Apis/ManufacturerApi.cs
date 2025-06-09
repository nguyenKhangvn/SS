using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Apis
{
    public static class ManufacturerApi
    {
        public static IEndpointRouteBuilder MapManufacturerApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0).RequireAuthorization();

            v1.MapGet("/manufacturers", async ([FromServices] IManufacturerService service) =>
            {
                var result = await service.GetAllAsync();
                return Results.Ok(result);
            });

            v1.MapGet("/manufacturers/{id:Guid}", async ([FromServices] IManufacturerService service, Guid id) =>
            {
                var result = await service.GetByIdAsync(id);
                return result is not null ? Results.Ok(result) : Results.NotFound();
            });

            v1.MapPost("/manufacturers", async ([FromServices] IManufacturerService service, [FromBody] ManufacturerDto dto) =>
            {
                var created = await service.CreateAsync(dto);
                return Results.Created($"/manufacturers/{created.Id}", created);
            });

            v1.MapPut("/manufacturers/{id:Guid}", async ([FromServices] IManufacturerService service, Guid id, [FromBody] ManufacturerDto dto) =>
            {
                var updated = await service.UpdateAsync(id, dto);
                return updated is not null ? Results.Ok(updated) : Results.NotFound();
            });

            v1.MapDelete("/manufacturers/{id:Guid}", async ([FromServices] IManufacturerService service, Guid id) =>
            {
                var success = await service.DeleteAsync(id);
                return success ? Results.Ok() : Results.NotFound();
            });

            return builder;
        }
    }
}
