using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
        public List<ChatParticipantDto> Participants { get; set; } = new List<ChatParticipantDto>();
    }

    // DTO for Message information
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }

    // DTO for Chat Participant information
    public class ChatParticipantDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    // DTO for tạo phòng chat
    public class CreateChatRequest
    {
        public string? Title { get; set; }
        public List<Guid> ParticipantUserIds { get; set; } = new List<Guid>();
    }

    // DTO for gửi tin nhắn
    public class SendMessageRequest
    {
        public Guid ChatId { get; set; }
        public string Content { get; set; }
    }

    // DTO for tham gia vào chat
    public class JoinChatRequest
    {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; } // User joining the chat
    }
} 