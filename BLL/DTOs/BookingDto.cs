using DAL.Models.Enum;

namespace BLL.DTOs
{
    public class BookingHomestayDetailDto
    {
        public Guid HomestayId { get; set; }
        public Guid? RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public decimal CleaningFee { get; set; }
        public decimal ServiceFee { get; set; }
        public bool HostApprovalRequired { get; set; }
    }

    public class BookingTourDetailDto
    {
        public Guid ScheduleId { get; set; }
        public int Participants { get; set; }
        public string? PickupLocation { get; set; }
        public TimeSpan? PickupTime { get; set; }
    }

    public class BookingDto
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? ComboId { get; set; }
        public string? BookingReference { get; set; }
        public BookingType BookingType { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int NumberOfGuests { get; set; }
        public string? SpecialRequests { get; set; }
        public string? VoucherCode { get; set; }
        public BookingHomestayDetailDto? HomestayDetail { get; set; }
        public BookingTourDetailDto? TourDetail { get; set; }
    }
}
