using DAL.Models.Enum;

namespace BLL.DTOs
{
    public class VoucherCreateDto
    {
        public Guid? UserId { get; set; }
        public string VoucherCode { get; set; } = string.Empty;
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public List<ServiceType> ServiceTypes { get; set; } = new();
        public bool IsPublic { get; set; }
        public int MaxUsage { get; set; }
    }

    public class VoucherDto
    {
        public Guid VoucherId { get; set; }
        public Guid? UserId { get; set; }
        public string VoucherCode { get; set; } = string.Empty;
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public List<ServiceType> ServiceTypes { get; set; } = new();
        public bool IsPublic { get; set; }
        public int MaxUsage { get; set; }
        public int CurrentUsage { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class VoucherValidationRequestDto
    {
        public Guid UserId { get; set; }
        public string VoucherCode { get; set; } = string.Empty;
        public decimal OriginalAmount { get; set; }
        public List<ServiceType> ServiceTypes { get; set; } = new();
    }

    public class VoucherValidationResultDto
    {
        public Guid VoucherId { get; set; }
        public string VoucherCode { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}
