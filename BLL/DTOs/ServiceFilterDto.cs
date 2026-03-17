using DAL.Models.Enum;

namespace BLL.DTOs
{
    public class ServiceFilterDto
    {
        public Guid? ServiceId { get; set; }
        public Guid? PartnerId { get; set; }
        public string? Title { get; set; }
        public Guid? LocationId { get; set; }
        public ServiceType? ServiceType { get; set; }
        public bool IncludeInactive { get; set; }
    }
}
