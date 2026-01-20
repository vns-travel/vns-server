namespace BLL.DTOs
{
    public class DestinationDto
    {
        public Guid DestinationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Description { get; set; }
    }

    public class DestinationImageDto
    {
        public Guid DestinationImageId { get; set; }
        public Guid DestinationId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? Caption { get; set; }
    }

    public class ServiceImageDto
    {
        public Guid ServiceImageId { get; set; }
        public Guid ServiceId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class BookingItemDto
    {
        public Guid BookingItemId { get; set; }
        public Guid BookingId { get; set; }
        public Guid ServiceId { get; set; }
        public Guid? ServiceDetailId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ItemStatus { get; set; }
    }

    public class ReviewDto
    {
        public Guid ReviewId { get; set; }
        public Guid BookingId { get; set; }
        public Guid? BookingItemId { get; set; }
        public Guid ServiceId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }

    public class NotificationDto
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int NotificationType { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public class AuthProviderDto
    {
        public Guid AuthProviderId { get; set; }
        public Guid UserId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
    }
}
