using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implementations
{
    public class BookingItemService : IBookingItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingItemDto>> GetByBookingIdAsync(Guid bookingId)
        {
            var items = await _unitOfWork.BookingItem.GetAllAsync(i => i.BookingId == bookingId);
            return _mapper.Map<IEnumerable<BookingItemDto>>(items);
        }

        public async Task<BookingItemDto?> GetByIdAsync(Guid bookingItemId)
        {
            var item = await _unitOfWork.BookingItem.GetAsync(i => i.BookingItemId == bookingItemId);
            return item == null ? null : _mapper.Map<BookingItemDto>(item);
        }

        public async Task<BookingItemDto> CreateAsync(BookingItemDto dto)
        {
            var entity = _mapper.Map<BookingItem>(dto);
            entity.BookingItemId = Guid.NewGuid();
            await _unitOfWork.BookingItem.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<BookingItemDto>(entity);
        }

        public async Task<bool> UpdateAsync(Guid bookingItemId, BookingItemDto dto)
        {
            var entity = await _unitOfWork.BookingItem.GetAsync(i => i.BookingItemId == bookingItemId);
            if (entity == null)
            {
                return false;
            }

            _mapper.Map(dto, entity);
            entity.BookingItemId = bookingItemId;
            entity.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.BookingItem.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid bookingItemId)
        {
            var entity = await _unitOfWork.BookingItem.GetAsync(i => i.BookingItemId == bookingItemId);
            if (entity == null)
            {
                return false;
            }

            await _unitOfWork.BookingItem.RemoveAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
