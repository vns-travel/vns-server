using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Destination
    {
        [Key]
        public Guid DestinationId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Country { get; set; }

        [StringLength(255)]
        public string? City { get; set; }

        [StringLength(255)]
        public string? District { get; set; }

        [StringLength(255)]
        public string? Ward { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<DestinationImage> DestinationImages { get; set; } = new HashSet<DestinationImage>();
        public virtual ICollection<Service> Services { get; set; } = new HashSet<Service>();
    }
}
