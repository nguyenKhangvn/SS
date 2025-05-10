using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Apis
{
    public static class UserApi
    {
        public static IEndpointRouteBuilder MapUserApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            // [POSt] http://localhost:5000/api/v1/ecommerce/users
            v1.MapPost("/users", (IUserService userService, UserCreateDto user) => userService.AddAsync(user));
            // [GET] http://localhost:5000/api/v1/ecommerce/users
            v1.MapGet("/users/{userId:guid}", async (IUserService service, Guid userId, [FromQuery] string? includeProperties = null) =>
            {
                var user = await service.GetByIdAsync(userId, includeProperties);
                return user is not null ? Results.Ok(user) : Results.NotFound();
            });
            // [GET] http://localhost:5000/api/v1/ecommerce/users
            v1.MapGet("/users", async (IUserService service, [FromQuery] string? includeProperties = null) =>
            {
                var users = await service.GetAllAsync();
                return Results.Ok(users);
            });
            // [PUT] http://localhost:5000/api/v1/ecommerce/users
            v1.MapPut("/users/{userId:guid}", async (IUserService service, Guid userId, UserCreateDto dto) =>
            {
                if (userId != dto.Id)
                {
                    return Results.BadRequest("ID trong URL không khớp với body");
                }

                var updated = await service.UpdateAsync(userId, dto);
                return Results.Ok(updated);
            });
            // [DELETE] http://localhost:5000/api/v1/ecommerce/users
            v1.MapDelete("/users/{userId:guid}", async (IUserService service, Guid userId) =>
            {
                var success = await service.DeleteAsync(userId);
                return success ? Results.Ok() : Results.NotFound();
            });

            //
            v1.MapGet("/users/email/{email}", async (IUserService service, string email) =>
            {
                var user = await service.GetByEmailAsync(email);
                return user is not null ? Results.Ok(user) : Results.NotFound();
            });

            v1.MapPut("users/update-info", async (IUserService service, [FromForm] UpdateInfoDto dto) =>
            {
                var user = await service.GetByIdAsync(dto.Id, null);
                if (user == null )
                {
                    return Results.NotFound();
                }
                var update = await service.UpdateInfoAsync(dto);
                
                return Results.Ok(update);
            })
            .DisableAntiforgery();
            
            return builder;
            
            
        }
    }
}
