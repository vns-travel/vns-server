using DAL.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public Guid? ComboId { get; set; }
        public Guid? VoucherId { get; set; }

        [Required]
        [StringLength(50)]
        public string BookingReference { get; set; }

        public int BookingType { get; set; } // 1: single_service, 2: combo
        public int BookingStatus { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal OriginalAmount { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal FinalAmount { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal DepositAmount { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal RemainingAmount { get; set; }

        public int PaymentMethod { get; set; }
        public int PaymentStatus { get; set; }

        public int NumberOfGuests { get; set; }

        public string SpecialRequests { get; set; }
        [StringLength(50)]
        public string? VoucherCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ComboId")]
        public virtual Combo? Combo { get; set; }

        [ForeignKey("VoucherId")]
        public virtual Voucher? Voucher { get; set; }

        public virtual ICollection<HomestayBooking> HomestayBookings { get; set; }
        public virtual ICollection<TourBooking> TourBookings { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Refund> Refunds { get; set; }
        public virtual ICollection<ServiceFeedback> ServiceFeedbacks { get; set; }
        public virtual ICollection<BookingItem> BookingItems { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
