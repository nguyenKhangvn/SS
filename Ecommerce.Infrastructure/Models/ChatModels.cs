namespace Ecommerce.Infrastructure.Models
{
    public class ChatRoom
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = default!;
        public string CustomerEmail { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }

    public class ChatMessage
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = default!;
        public DateTime SentAt { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = default!;
        public bool IsStaff { get; set; }
        public Guid RoomId { get; set; }
        public ChatRoom Room { get; set; } = default!;
    }
} 