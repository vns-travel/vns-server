using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Conversation
    {
        [Key]
        public Guid ConversationId { get; set; }

        [Required]
        public Guid UserAId { get; set; }

        [Required]
        public Guid UserBId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
