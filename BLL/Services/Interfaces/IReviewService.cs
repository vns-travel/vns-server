using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllAsync();
        Task<IEnumerable<ReviewDto>> GetByServiceIdAsync(Guid serviceId);
        Task<IEnumerable<ReviewDto>> GetByBookingIdAsync(Guid bookingId);
        Task<ReviewDto?> GetByIdAsync(Guid reviewId);
        Task<ReviewDto> CreateAsync(ReviewDto dto);
        Task<bool> UpdateAsync(Guid reviewId, ReviewDto dto);
        Task<bool> DeleteAsync(Guid reviewId);
    }
}
