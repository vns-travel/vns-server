using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IServiceImageService
    {
        Task<IEnumerable<ServiceImageDto>> GetByServiceIdAsync(Guid serviceId);
        Task<ServiceImageDto?> GetByIdAsync(Guid serviceImageId);
        Task<ServiceImageDto> CreateAsync(ServiceImageDto dto);
        Task<bool> DeleteAsync(Guid serviceImageId);
    }
}
