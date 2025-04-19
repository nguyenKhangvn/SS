using System.Security.Claims;

namespace Ecommerce.API.Services.Interfaces
{
    public interface ITokenService
    {
        // Tạo JWT token
        string GenerateJwtToken(User user);

        // Tạo refresh token ngẫu nhiên
        string GenerateRefreshToken();

        // Xác thực JWT token
        ClaimsPrincipal? ValidateToken(string token);

        // Lấy thông tin user từ expired token (dùng cho refresh token flow)
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

        // Lấy thời hạn token từ cấu hình
        DateTime GetAccessTokenExpiry();
    }
}
