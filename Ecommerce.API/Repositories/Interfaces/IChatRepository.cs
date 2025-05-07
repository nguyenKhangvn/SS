using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IChatRepository
    {
        Task<Chat?> GetByIdAsync(Guid id);
        Task<IEnumerable<Chat>> GetChatsForUserAsync(Guid userId);
        Task<Chat> AddAsync(Chat chat);
        Task UpdateAsync(Chat chat);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<Chat?> FindChatByCustomerIdAsync(Guid customerId);
      //  Task<Chat?> FindOrCreateDirectChatAsync(Guid userId1, Guid userId2);
        Task<IEnumerable<ChatConversationDto>> GetUserConversationsAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
    }
} 