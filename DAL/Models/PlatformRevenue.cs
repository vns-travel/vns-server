using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class PlatformRevenue
    {
        [Key]
        public Guid PlatformRevenueId { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalGross { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal PlatformFee { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal NetRevenue { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
