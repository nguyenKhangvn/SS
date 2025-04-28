using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Repositories
{
    public interface IChatRepository
    {
        Task<Chat?> GetByIdAsync(Guid id);
        Task<IEnumerable<Chat>> GetChatsForUserAsync(Guid userId);
        Task AddAsync(Chat chat);
        Task UpdateAsync(Chat chat);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
} 