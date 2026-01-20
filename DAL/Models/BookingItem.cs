using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class BookingItem
    {
        [Key]
        public Guid BookingItemId { get; set; }

        [Required]
        public Guid BookingId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        /// <summary>
        /// Optional link to concrete service subtype (hotel/tour/car) if needed.
        /// </summary>
        public Guid? ServiceDetailId { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; } = 1;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Status per item (align to new schema). Stored as int for flexibility.
        /// </summary>
        public int ItemStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(BookingId))]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey(nameof(ServiceId))]
        public virtual Service Service { get; set; } = null!;
    }
}
