using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implementations
{
    public class PartnerService : IPartnerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PartnerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PartnerServiceResponseDto> CreateAsync(Guid userId, PartnerServiceCreateDto dto)
        {
            var partner = await _unitOfWork.Partner.GetAsync(p => p.UserId == userId);
            if (partner == null)
            {
                throw new KeyNotFoundException("Partner not found");
            }

            var location = await _unitOfWork.Location.GetAsync(l => l.LocationId == dto.LocationId);
            if (location == null)
            {
                throw new KeyNotFoundException("Location not found");
            }

            if (dto.DestinationId.HasValue)
            {
                var destination = await _unitOfWork.Destination.GetAsync(d => d.DestinationId == dto.DestinationId.Value);
                if (destination == null)
                {
                    throw new KeyNotFoundException("Destination not found");
                }
            }

            var service = new Service
            {
                ServiceId = Guid.NewGuid(),
                PartnerId = partner.PartnerId,
                LocationId = dto.LocationId,
                DestinationId = dto.DestinationId,
                ServiceType = (int)dto.ServiceType,
                Title = dto.Title,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Service.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();

            return MapResponse(service);
        }

        public async Task<PartnerServiceResponseDto> UpdateAsync(Guid userId, Guid serviceId, PartnerServiceUpdateDto dto)
        {
            var partner = await _unitOfWork.Partner.GetAsync(p => p.UserId == userId);
            if (partner == null)
            {
                throw new KeyNotFoundException("Partner not found");
            }

            var service = await _unitOfWork.Service.GetAsync(s => s.ServiceId == serviceId, tracked: true);
            if (service == null)
            {
                throw new KeyNotFoundException("Service not found");
            }

            if (service.PartnerId != partner.PartnerId)
            {
                throw new UnauthorizedAccessException("Service does not belong to partner");
            }

            var location = await _unitOfWork.Location.GetAsync(l => l.LocationId == dto.LocationId);
            if (location == null)
            {
                throw new KeyNotFoundException("Location not found");
            }

            if (dto.DestinationId.HasValue)
            {
                var destination = await _unitOfWork.Destination.GetAsync(d => d.DestinationId == dto.DestinationId.Value);
                if (destination == null)
                {
                    throw new KeyNotFoundException("Destination not found");
                }
            }

            service.LocationId = dto.LocationId;
            service.DestinationId = dto.DestinationId;
            service.ServiceType = (int)dto.ServiceType;
            service.Title = dto.Title;
            service.Description = dto.Description;
            service.IsActive = dto.IsActive;
            service.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Service.UpdateAsync(service);
            await _unitOfWork.SaveChangesAsync();

            return MapResponse(service);
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid serviceId)
        {
            var partner = await _unitOfWork.Partner.GetAsync(p => p.UserId == userId);
            if (partner == null)
            {
                throw new KeyNotFoundException("Partner not found");
            }

            var service = await _unitOfWork.Service.GetAsync(s => s.ServiceId == serviceId, tracked: true);
            if (service == null)
            {
                return false;
            }

            if (service.PartnerId != partner.PartnerId)
            {
                throw new UnauthorizedAccessException("Service does not belong to partner");
            }

            await _unitOfWork.Service.RemoveAsync(service);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static PartnerServiceResponseDto MapResponse(Service service)
        {
            return new PartnerServiceResponseDto
            {
                ServiceId = service.ServiceId,
                PartnerId = service.PartnerId,
                LocationId = service.LocationId,
                DestinationId = service.DestinationId,
                ServiceType = (DAL.Models.Enum.ServiceType)service.ServiceType,
                Title = service.Title,
                Description = service.Description,
                IsActive = service.IsActive,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt
            };
        }
    }
}
