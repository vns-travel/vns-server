using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IBookingItemService
    {
        Task<IEnumerable<BookingItemDto>> GetByBookingIdAsync(Guid bookingId);
        Task<BookingItemDto?> GetByIdAsync(Guid bookingItemId);
        Task<BookingItemDto> CreateAsync(BookingItemDto dto);
        Task<bool> UpdateAsync(Guid bookingItemId, BookingItemDto dto);
        Task<bool> DeleteAsync(Guid bookingItemId);
    }
}
