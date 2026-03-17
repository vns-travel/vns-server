using BLL.DTOs;
using DAL.Models.Enum;

namespace BLL.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<VoucherDto> CreateAsync(Guid managerUserId, VoucherCreateDto dto);
        Task<IEnumerable<VoucherDto>> GetAvailableVouchersAsync(Guid userId);
        Task<VoucherValidationResultDto?> ValidateAsync(Guid userId, string voucherCode, decimal originalAmount, IEnumerable<ServiceType> serviceTypes);
        Task IssueWelcomeVoucherAsync(Guid userId);
        Task IncrementUsageAsync(Guid voucherId);
    }
}
