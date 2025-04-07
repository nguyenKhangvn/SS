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

            v1.MapPost("/users", (IUserService userService, UserCreateDto user) => userService.AddAsync(user));
            v1.MapGet("/users/{userId:guid}", async (IUserService service, Guid userId, [FromQuery] string? includeProperties = null) =>
            {
                var user = await service.GetByIdAsync(userId, includeProperties);
                return user is not null ? Results.Ok(user) : Results.NotFound();
            });
            v1.MapGet("/users", async (IUserService service, [FromQuery] string? includeProperties = null) =>
            {
                var users = await service.GetAllAsync();
                return Results.Ok(users);
            });
            v1.MapPut("/users/{userId:guid}", async (IUserService service, Guid userId, UserCreateDto dto) =>
            {
                if (userId != dto.Id)
                {
                    return Results.BadRequest("ID trong URL không khớp với body");
                }

                var updated = await service.UpdateAsync(userId, dto);
                return Results.Ok(updated);
            });
            v1.MapDelete("/users/{userId:guid}", async (IUserService service, Guid userId) =>
            {
                var success = await service.DeleteAsync(userId);
                return success ? Results.Ok() : Results.NotFound();
            });
            v1.MapPost("/users/register", async (UserCreateDto dto, IUserService service) =>
            {
                var result = await service.RegisterAsync(dto);
                return result == null ? Results.BadRequest("Email đã tồn tại.") : Results.Ok(result);
            });

            v1.MapPost("/users/login", async (UserLoginDto dto, IUserService service) =>
            {
                var result = await service.LoginAsync(dto);
                return result == null ? Results.Unauthorized() : Results.Ok(result);
            });
            return builder;
        }
    }
}
