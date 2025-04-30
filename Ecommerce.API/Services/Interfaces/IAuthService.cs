using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity.Data;
using static Ecommerce.Infrastructure.Models.Dtos.AuthDto;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequest dto);
        Task<AuthResponseDto> RegisterAsync(UserCreateDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task RevokeTokenAsync(string refreshToken);
        Task RequestPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
        Task<AuthResponseDto> LoginWithGoogleAsync(string email, string name);
    }
}
