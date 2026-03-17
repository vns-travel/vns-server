using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IPartnerService
    {
        Task<PartnerServiceResponseDto> CreateAsync(Guid userId, PartnerServiceCreateDto dto);
        Task<PartnerServiceResponseDto> UpdateAsync(Guid userId, Guid serviceId, PartnerServiceUpdateDto dto);
        Task<PartnerServiceResponseDto> ApproveAsync(Guid serviceId);
        Task<bool> DeleteAsync(Guid userId, Guid serviceId);
    }
}
