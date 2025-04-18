namespace WebApplication_AuthenticationSystem_.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? MessageText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
