using Microsoft.Extensions.Caching.Memory;
using static Ecommerce.Infrastructure.Models.Dtos.AuthDto;

namespace Ecommerce.API.Services
{
    public class TempCodeMemoryStoreService : ITempCodeStore
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(5); // thời gian sống của code

        public TempCodeMemoryStoreService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task SaveAsync(string code, AuthResponseDto data, TimeSpan? expiry = null)
        {
            _cache.Set(code, data, expiry ?? _defaultExpiry);
            return Task.CompletedTask;
        }

        public Task<AuthResponseDto?> GetAsync(string code)
        {
            _cache.TryGetValue(code, out AuthResponseDto? data);
            return Task.FromResult(data);
        }

        public Task RemoveAsync(string code)
        {
            _cache.Remove(code);
            return Task.CompletedTask;
        }
    }
}
