using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IDestinationImageService
    {
        Task<IEnumerable<DestinationImageDto>> GetByDestinationIdAsync(Guid destinationId);
        Task<DestinationImageDto?> GetByIdAsync(Guid destinationImageId);
        Task<DestinationImageDto> CreateAsync(DestinationImageDto dto);
        Task<bool> DeleteAsync(Guid destinationImageId);
    }
}
