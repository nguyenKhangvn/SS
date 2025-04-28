namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IChatParticipantRepository
    {
        Task AddAsync(ChatParticipant participant);
        Task<ChatParticipant?> GetByChatAndUserAsync(Guid chatId, Guid userId);
        Task<IEnumerable<ChatParticipant>> GetParticipantsForChatAsync(Guid chatId);
        Task<bool> IsUserInChatAsync(Guid chatId, Guid userId);
    }
}
    