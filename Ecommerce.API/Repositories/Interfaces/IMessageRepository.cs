namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message?> GetByIdAsync(Guid id);
        Task<IEnumerable<Message>> GetMessagesForChatAsync(Guid chatId, int skip = 0, int take = 50);
        Task<Message> AddAsync(Message message);
        Task<bool> UpdateAsync(Message message);
        Task<bool> DeleteAsync(Guid id);
        Task<int> GetUnreadCountAsync(Guid chatId, Guid userId);
        Task<bool> MarkMessagesAsReadAsync(Guid chatId, Guid userId);
    }
} 