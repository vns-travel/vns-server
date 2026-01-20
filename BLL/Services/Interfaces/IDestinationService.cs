using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IDestinationService
    {
        Task<IEnumerable<DestinationDto>> GetAllAsync();
        Task<DestinationDto?> GetByIdAsync(Guid destinationId);
        Task<DestinationDto> CreateAsync(DestinationDto dto);
        Task<bool> UpdateAsync(Guid destinationId, DestinationDto dto);
        Task<bool> DeleteAsync(Guid destinationId);
    }
}
