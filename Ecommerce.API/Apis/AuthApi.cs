using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity.Data;
using static Ecommerce.Infrastructure.Models.Dtos.AuthDto;

namespace Ecommerce.API.Apis
{
    public static class AuthApi
    {
        public static IEndpointRouteBuilder MapAuthApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            // [POSt] http://localhost:5000/api/v1/ecommerce/users
            v1.MapPost("/auth/login", async (LoginRequest dto, IAuthService service) =>
            {
                try
                {
                    var result = await service.LoginAsync(dto);
                    return Results.Ok(result);
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Unauthorized();
                }
            });

            v1.MapPost("/auth/register", async (UserCreateDto dto, IAuthService service) =>
            {
                try
                {
                    var result = await service.RegisterAsync(dto);
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });
            v1.MapPost("/auth/request-password-reset", async (string email, IAuthService service) =>
            {
                await service.RequestPasswordResetAsync(email);
                return Results.Ok();
            });

            v1.MapPost("/auth/reset-password", async (ResetPasswordDto dto, IAuthService service) =>
            {
                var success = await service.ResetPasswordAsync(dto);
                return success ? Results.Ok() : Results.BadRequest();
            });
            v1.MapPost("/auth/revoke-token", async (RefreshTokenRequestDto dto, IAuthService service) =>
            {
                await service.RevokeTokenAsync(dto.RefreshToken);
                return Results.Ok(new { message = "Token revoked" });
            });


            return builder;
        }
    }
}
