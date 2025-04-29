using static Ecommerce.Infrastructure.Models.Dtos.AuthDto;

namespace Ecommerce.API.Services.Interfaces
{
    public interface ITempCodeStore
    {
        Task SaveAsync(string code, AuthResponseDto data, TimeSpan? expiry = null);
        Task<AuthResponseDto?> GetAsync(string code);
        Task RemoveAsync(string code);
    }
}
