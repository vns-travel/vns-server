using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class AdminLog
    {
        [Key]
        public Guid AdminLogId { get; set; }

        [Required]
        [StringLength(255)]
        public string Action { get; set; } = string.Empty;

        [StringLength(255)]
        public string? PerformedBy { get; set; }

        [StringLength(2000)]
        public string? Details { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
