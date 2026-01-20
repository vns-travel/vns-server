using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class ServiceImage
    {
        [Key]
        public Guid ServiceImageId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Caption { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ServiceId))]
        public virtual Service Service { get; set; } = null!;
    }
}
