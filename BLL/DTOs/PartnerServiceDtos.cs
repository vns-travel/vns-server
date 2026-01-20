using DAL.Models.Enum;

namespace BLL.DTOs
{
    public class PartnerServiceCreateDto
    {
        public Guid LocationId { get; set; }
        public Guid? DestinationId { get; set; }
        public ServiceType ServiceType { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PartnerServiceUpdateDto
    {
        public Guid LocationId { get; set; }
        public Guid? DestinationId { get; set; }
        public ServiceType ServiceType { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PartnerServiceResponseDto
    {
        public Guid ServiceId { get; set; }
        public Guid PartnerId { get; set; }
        public Guid LocationId { get; set; }
        public Guid? DestinationId { get; set; }
        public ServiceType ServiceType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
