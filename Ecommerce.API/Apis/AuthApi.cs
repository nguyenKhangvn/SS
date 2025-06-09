using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System;
using static Ecommerce.Infrastructure.Models.Dtos.AuthDto;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Ecommerce.API.Services.Interfaces;

namespace Ecommerce.API.Apis
{
    public static class AuthApi
    {
        public static string DefaultFeLoginRedirectUri =>
        Environment.GetEnvironmentVariable("DEFAULT_FE_LOGIN_REDIRECT_URI") ?? "http://localhost:5173/login";

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
                var refreshToken = context.Request.Cookies["refresh_token"];
                var accessToken = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
                    return Results.Unauthorized();

                try
                {
                    var result = await service.RefreshTokenAsync(new RefreshTokenRequestDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    });

                    // Cập nhật refresh token mới trong cookie
                    context.Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = result.Expiry
                    });

                    return Results.Ok(new
                    {
                        AccessToken = result.AccessToken,
                        Expiry = result.Expiry
                    });
                }
                catch (SecurityTokenException)
                {
                    context.Response.Cookies.Delete("refresh_token");
                    return Results.Unauthorized();
                }
            }).RequireAuthorization();

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
            v1.MapGet("/auth/login-google", async (HttpContext context) =>
            {
                var redirectUri = context.Request.Query["redirect_uri"].ToString();

                if (string.IsNullOrEmpty(redirectUri))
                    redirectUri = DefaultFeLoginRedirectUri;

                var props = new AuthenticationProperties
                {
                    RedirectUri = $"/api/v1/ecommerce/auth/google-response?redirect_uri={Uri.EscapeDataString(redirectUri)}"
                };

                await context.ChallengeAsync("Google", props);
            });


            v1.MapGet("/auth/google-response", async (
                     HttpContext context,
                     [FromServices] IAuthService authService,
                     [FromServices] ITempCodeStore tempCodeStore) =>
            {
                var authResult = await context.AuthenticateAsync("Google");
                if (!authResult.Succeeded)
                    return Results.Redirect(GetErrorRedirect(context, "auth_failed"));

                var email = authResult.Principal.FindFirstValue(ClaimTypes.Email);
                var name = authResult.Principal.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrEmpty(email))
                    return Results.Redirect(GetErrorRedirect(context, "no_email"));

                try
                {
                    var response = await authService.LoginWithGoogleAsync(email, name);
                    var tempCode = Guid.NewGuid().ToString("N");
                    await tempCodeStore.SaveAsync(tempCode, response);

                    var redirectUri = context.Request.Query["redirect_uri"].ToString();
                    if (string.IsNullOrEmpty(redirectUri))
                        redirectUri = DefaultFeLoginRedirectUri;

                    return Results.Redirect($"{redirectUri}?code={tempCode}");
                }
                catch
                {
                    return Results.Redirect(GetErrorRedirect(context, "404"));
                }
            });


            v1.MapPost("/auth/google-callback", async (
                [FromBody] GoogleCallbackRequestDto dto,
                [FromServices] ITempCodeStore store) =>
            {
                var authResponse = await store.GetAsync(dto.Code);
                if (authResponse == null)
                    return Results.BadRequest("!!!code");

                await store.RemoveAsync(dto.Code);

                return Results.Ok(authResponse);
            });
            return builder;
        }
        private static string GetErrorRedirect(HttpContext context, string error)
        {
            // Lấy redirect_uri từ query string của request hiện tại
            var redirectUriFromQuery = context.Request.Query["redirect_uri"].FirstOrDefault();

            // Sử dụng redirect_uri từ query nếu có, nếu không thì dùng DefaultFeLoginRedirectUri
            var finalRedirectUri = string.IsNullOrEmpty(redirectUriFromQuery) ? DefaultFeLoginRedirectUri : redirectUriFromQuery;

            return $"{finalRedirectUri}?error={error}";
        }

        private static string GetRedirectUri(this HttpContext context)
        {
            return context.Request.Query["redirect_uri"].FirstOrDefault()
                   ?? "http://localhost:5173/";
        }
    }
}
