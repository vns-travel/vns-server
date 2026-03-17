using BLL.Services.Interfaces;
using BLL.DTOs.Tour;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implementations
{
    public class TourServiceService : ITourService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TourServiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ICollection<TourService>> GetAllAsync() => await _unitOfWork.TourService.GetAllAsync();
        public async Task<TourService?> GetByIdAsync(Guid id) => await _unitOfWork.TourService.GetAsync(x => x.TourId == id);
        public async Task AddAsync(TourService entity) { await _unitOfWork.TourService.AddAsync(entity); await _unitOfWork.SaveChangesAsync(); }
        public async Task UpdateAsync(TourService entity) { await _unitOfWork.TourService.UpdateAsync(entity); await _unitOfWork.SaveChangesAsync(); }
        public async Task DeleteAsync(Guid id) { var entity = await _unitOfWork.TourService.GetAsync(x => x.TourId == id); if (entity != null) { await _unitOfWork.TourService.RemoveAsync(entity); await _unitOfWork.SaveChangesAsync(); } }

        public async Task<TourScheduleResponseDto> CreateScheduleAsync(Guid userId, Guid tourId, CreateTourScheduleDto dto)
        {
            var partner = await _unitOfWork.Partner.GetAsync(p => p.UserId == userId);
            if (partner == null)
            {
                throw new KeyNotFoundException("Partner not found");
            }

            var tour = await _unitOfWork.TourService.GetAsync(t => t.TourId == tourId, includeProperties: "Service");
            if (tour == null || tour.Service == null)
            {
                throw new KeyNotFoundException("Tour not found");
            }

            if (tour.Service.PartnerId != partner.PartnerId)
            {
                throw new UnauthorizedAccessException("Tour does not belong to partner");
            }

            if (dto.AvailableSlots <= 0)
            {
                throw new InvalidOperationException("Available slots must be greater than zero.");
            }

            if (dto.Price <= 0)
            {
                throw new InvalidOperationException("Schedule price must be greater than zero.");
            }

            var schedule = new TourSchedule
            {
                ScheduleId = Guid.NewGuid(),
                TourId = tourId,
                TourDate = dto.TourDate.Date,
                StartTime = TimeSpan.Parse(dto.StartTime),
                EndTime = TimeSpan.Parse(dto.EndTime),
                AvailableSlots = dto.AvailableSlots,
                BookedSlots = 0,
                GuideId = dto.GuideId ?? string.Empty,
                MeetingPoint = dto.MeetingPoint ?? string.Empty,
                IsActive = dto.IsActive,
                Price = dto.Price
            };

            await _unitOfWork.TourSchedule.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            return MapSchedule(schedule);
        }

        public async Task<TourItineraryResponseDto> CreateItineraryAsync(Guid userId, Guid tourId, CreateTourItineraryDto dto)
        {
            var partner = await _unitOfWork.Partner.GetAsync(p => p.UserId == userId);
            if (partner == null)
            {
                throw new KeyNotFoundException("Partner not found");
            }

            var tour = await _unitOfWork.TourService.GetAsync(t => t.TourId == tourId, includeProperties: "Service");
            if (tour == null || tour.Service == null)
            {
                throw new KeyNotFoundException("Tour not found");
            }

            if (tour.Service.PartnerId != partner.PartnerId)
            {
                throw new UnauthorizedAccessException("Tour does not belong to partner");
            }

            var itinerary = new TourItinerary
            {
                ItineraryId = Guid.NewGuid(),
                TourId = tourId,
                StepOrder = dto.StepOrder,
                Location = dto.Location,
                Activity = dto.Activity,
                DurationMinutes = dto.DurationMinutes,
                Description = dto.Description
            };

            await _unitOfWork.TourItinerary.AddAsync(itinerary);
            await _unitOfWork.SaveChangesAsync();

            return new TourItineraryResponseDto
            {
                ItineraryId = itinerary.ItineraryId,
                TourId = itinerary.TourId,
                StepOrder = itinerary.StepOrder,
                Location = itinerary.Location,
                Activity = itinerary.Activity,
                DurationMinutes = itinerary.DurationMinutes,
                Description = itinerary.Description
            };
        }

        private static TourScheduleResponseDto MapSchedule(TourSchedule schedule)
        {
            return new TourScheduleResponseDto
            {
                ScheduleId = schedule.ScheduleId,
                TourId = schedule.TourId,
                TourDate = schedule.TourDate,
                StartTime = schedule.StartTime.ToString(),
                EndTime = schedule.EndTime.ToString(),
                AvailableSlots = schedule.AvailableSlots,
                BookedSlots = schedule.BookedSlots,
                GuideId = schedule.GuideId,
                MeetingPoint = schedule.MeetingPoint,
                IsActive = schedule.IsActive,
                Price = schedule.Price
            };
        }
    }
} 