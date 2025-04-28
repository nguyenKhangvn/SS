namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessagesForChatAsync(Guid chatId, int skip = 0, int take = 50);
        Task AddAsync(Message message);
        Task<bool> ExistsAsync(Guid id);
    }
}
