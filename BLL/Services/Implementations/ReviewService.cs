using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllAsync()
        {
            var reviews = await _unitOfWork.Review.GetAllAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetByServiceIdAsync(Guid serviceId)
        {
            var reviews = await _unitOfWork.Review.GetAllAsync(r => r.ServiceId == serviceId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetByBookingIdAsync(Guid bookingId)
        {
            var reviews = await _unitOfWork.Review.GetAllAsync(r => r.BookingId == bookingId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> GetByIdAsync(Guid reviewId)
        {
            var review = await _unitOfWork.Review.GetAsync(r => r.ReviewId == reviewId);
            return review == null ? null : _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewDto> CreateAsync(ReviewDto dto)
        {
            var entity = _mapper.Map<Review>(dto);
            entity.ReviewId = Guid.NewGuid();
            await _unitOfWork.Review.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ReviewDto>(entity);
        }

        public async Task<bool> UpdateAsync(Guid reviewId, ReviewDto dto)
        {
            var entity = await _unitOfWork.Review.GetAsync(r => r.ReviewId == reviewId);
            if (entity == null)
            {
                return false;
            }

            _mapper.Map(dto, entity);
            entity.ReviewId = reviewId;
            entity.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Review.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid reviewId)
        {
            var entity = await _unitOfWork.Review.GetAsync(r => r.ReviewId == reviewId);
            if (entity == null)
            {
                return false;
            }

            await _unitOfWork.Review.RemoveAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
