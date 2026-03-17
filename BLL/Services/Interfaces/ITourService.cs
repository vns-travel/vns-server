using BLL.DTOs.Tour;
using DAL.Models;

namespace BLL.Services.Interfaces
{
    public interface ITourService
    {
        Task<ICollection<TourService>> GetAllAsync();
        Task<TourService?> GetByIdAsync(Guid id);
        Task AddAsync(TourService entity);
        Task UpdateAsync(TourService entity);
        Task DeleteAsync(Guid id);
        Task<TourScheduleResponseDto> CreateScheduleAsync(Guid userId, Guid tourId, CreateTourScheduleDto dto);
        Task<TourItineraryResponseDto> CreateItineraryAsync(Guid userId, Guid tourId, CreateTourItineraryDto dto);
    }
} 