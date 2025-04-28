using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System;
using static Ecommerce.Infrastructure.Models.Dtos.AuthDto;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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
            //login with gg
          
            // Login route
            v1.MapGet("/login-google", async (HttpContext context) =>
            {
                var props = new AuthenticationProperties
                {
                    RedirectUri = "/google-response" // Sau khi login xong, quay về đây
                };
                await context.ChallengeAsync("Google", props);
            });

            // Callback sau khi Google xác thực
            v1.MapGet("/google-response", async (HttpContext context, IAuthService authService) =>
            {
                var authenticateResult = await context.AuthenticateAsync("Google");

                if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
                {
                    return Results.Unauthorized();
                }

                var email = authenticateResult.Principal.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
                var name = authenticateResult.Principal.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return Results.BadRequest("Email not found from Google");
                }

                // Gọi vào AuthService để xử lý login (giống như login thường)
                var authResponse = await authService.LoginWithGoogleAsync(email, name);

                // Sau khi login thành công, quay lại frontend (FE)
                var frontendRedirectUrl = "http://localhost:5173/";  // URL của FE sau khi login thành công

                // Bạn có thể truyền token hoặc thông tin khác vào URL để frontend sử dụng
                frontendRedirectUrl = $"{frontendRedirectUrl}?access_token={authResponse.AccessToken}";

                // Redirect về frontend
                return Results.Redirect(frontendRedirectUrl);
            });



            return builder;
        }
    }
}
