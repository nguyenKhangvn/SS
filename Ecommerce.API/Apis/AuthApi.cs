using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System;
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

            v1.MapPost("/auth/logout", async (HttpContext context, IAuthService authService) =>
            {
                var refreshToken = context.Request.Cookies["refresh_token"]; // Không đọc từ body

                //if (string.IsNullOrEmpty(refreshToken))
                //    return Results.BadRequest("No session found");

                await authService.RevokeTokenAsync(refreshToken);

                context.Response.Cookies.Delete("refresh_token", new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
                return Results.Ok();
            });
            // [POST] http://localhost:5000/api/v1/ecommerce/auth/refresh-token
            v1.MapPost("/auth/refresh-token", async (HttpContext context, IAuthService service) =>
            {
                try
                {
                    // 1. Lấy refresh token từ cookie
                    var refreshTokenFromCookie = context.Request.Cookies["refresh_token"];
                    if (string.IsNullOrEmpty(refreshTokenFromCookie))
                    {
                        context.Response.Cookies.Delete("refresh_token");
                        return Results.Unauthorized();
                    }

                    // 2. Đọc access token từ body request
                    var requestDto = await context.Request.ReadFromJsonAsync<RefreshTokenRequestDto>();
                    if (requestDto == null || string.IsNullOrEmpty(requestDto.AccessToken))
                    {
                        return Results.BadRequest("Access token is required in request body");
                    }

                    // 3. Tạo DTO để gọi service (kết hợp token từ cookie và body)
                    var refreshRequest = new RefreshTokenRequestDto
                    {
                        AccessToken = requestDto.AccessToken,
                        RefreshToken = refreshTokenFromCookie
                    };

                    // 4. Gọi service xử lý nghiệp vụ
                    var authResponse = await service.RefreshTokenAsync(refreshRequest);

                    // 5. Cập nhật cookie với refresh token mới
                    context.Response.Cookies.Append(
                        "refresh_token",
                        authResponse.RefreshToken,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = authResponse.Expiry, // Sử dụng thời gian hết hạn từ service
                            Path = "/"
                        });

                    // 6. Trả về response (không bao gồm refresh token trong body)
                    var responseToClient = new
                    {
                        AccessToken = authResponse.AccessToken,
                        Expiry = authResponse.Expiry,
                        User = authResponse.User
                    };

                    return Results.Ok(responseToClient);
                }
                catch (SecurityTokenException ex)
                {
                    context.Response.Cookies.Delete("refresh_token");
                    return Results.Unauthorized();
                }
                catch (Exception ex)
                {
                    // Log lỗi ở đây nếu cần
                    return Results.Problem(
                        detail: "An unexpected error occurred",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            });
            //validate token
            v1.MapGet("/auth/validate-token", (HttpContext context, ITokenService tokenService) =>
            {
                var authorizationHeader = context.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                {
                    return Results.Unauthorized();
                }

                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var principal = tokenService.ValidateToken(token);

                if (principal == null)
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(new { message = "Token is valid" });
            });

            return builder;
        }
    }
}
