using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.Enum;
using DAL.Repositories.Interfaces;
using System.Text.Json;

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

            if (dto.ServiceType == ServiceType.Homestay)
            {
                throw new InvalidOperationException("Use the partner homestay flow to create homestay services.");
            }

            ValidateServiceRequest(dto.ServiceType, dto.PlatformFeeAmount, dto.TourDetails);

            var service = new Service
            {
                ServiceId = Guid.NewGuid(),
                PartnerId = partner.PartnerId,
                LocationId = dto.LocationId,
                DestinationId = dto.DestinationId,
                ServiceType = (int)dto.ServiceType,
                Title = dto.Title,
                Description = dto.Description,
                PlatformFeeAmount = dto.ServiceType == ServiceType.Other ? dto.PlatformFeeAmount : 0,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Service.AddAsync(service);

            if (dto.ServiceType == ServiceType.Tour)
            {
                await _unitOfWork.TourService.AddAsync(MapTour(service.ServiceId, dto.TourDetails!));
            }

            await _unitOfWork.SaveChangesAsync();

            return await BuildResponseAsync(service.ServiceId);
        }

        public async Task<PartnerServiceResponseDto> UpdateAsync(Guid userId, Guid serviceId, PartnerServiceUpdateDto dto)
        {
            var partner = await _unitOfWork.Partner.GetAsync(p => p.UserId == userId);
            if (partner == null)
            {
                throw new KeyNotFoundException("Partner not found");
            }

            var service = await _unitOfWork.Service.GetAsync(s => s.ServiceId == serviceId, includeProperties: "TourService", tracked: true);
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

            if (service.ServiceType == (int)ServiceType.Homestay || dto.ServiceType == ServiceType.Homestay)
            {
                throw new InvalidOperationException("Use the partner homestay flow to update homestay services.");
            }

            ValidateServiceRequest(dto.ServiceType, dto.PlatformFeeAmount, dto.TourDetails);

            service.LocationId = dto.LocationId;
            service.DestinationId = dto.DestinationId;
            service.ServiceType = (int)dto.ServiceType;
            service.Title = dto.Title;
            service.Description = dto.Description;
            service.PlatformFeeAmount = dto.ServiceType == ServiceType.Other ? dto.PlatformFeeAmount : 0;
            service.IsActive = false;
            service.UpdatedAt = DateTime.UtcNow;

            var existingTour = await _unitOfWork.TourService.GetAsync(t => t.ServiceId == serviceId, tracked: true);
            if (dto.ServiceType == ServiceType.Tour)
            {
                if (existingTour == null)
                {
                    await _unitOfWork.TourService.AddAsync(MapTour(service.ServiceId, dto.TourDetails!));
                }
                else
                {
                    UpdateTour(existingTour, dto.TourDetails!);
                    await _unitOfWork.TourService.UpdateAsync(existingTour);
                }
            }
            else if (existingTour != null)
            {
                await _unitOfWork.TourService.RemoveAsync(existingTour);
            }

            await _unitOfWork.Service.UpdateAsync(service);
            await _unitOfWork.SaveChangesAsync();

            return await BuildResponseAsync(service.ServiceId);
        }

        public async Task<PartnerServiceResponseDto> ApproveAsync(Guid serviceId)
        {
            var service = await _unitOfWork.Service.GetAsync(s => s.ServiceId == serviceId, tracked: true);
            if (service == null)
            {
                throw new KeyNotFoundException("Service not found");
            }

            service.IsActive = true;
            service.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Service.UpdateAsync(service);
            await _unitOfWork.SaveChangesAsync();

            return await BuildResponseAsync(service.ServiceId);
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

            var tour = await _unitOfWork.TourService.GetAsync(t => t.ServiceId == serviceId, tracked: true);
            if (tour != null)
            {
                await _unitOfWork.TourService.RemoveAsync(tour);
            }

            await _unitOfWork.Service.RemoveAsync(service);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async Task<PartnerServiceResponseDto> BuildResponseAsync(Guid serviceId)
        {
            var service = await _unitOfWork.Service.GetAsync(s => s.ServiceId == serviceId, includeProperties: "TourService");
            if (service == null)
            {
                throw new KeyNotFoundException("Service not found");
            }

            return new PartnerServiceResponseDto
            {
                ServiceId = service.ServiceId,
                TourId = service.TourService?.TourId,
                PartnerId = service.PartnerId,
                LocationId = service.LocationId,
                DestinationId = service.DestinationId,
                ServiceType = (ServiceType)service.ServiceType,
                Title = service.Title,
                Description = service.Description,
                PlatformFeeAmount = service.PlatformFeeAmount,
                IsActive = service.IsActive,
                PublicationMode = service.ServiceType == (int)ServiceType.Other ? "News" : "Service",
                TourDetails = service.TourService == null ? null : new PartnerTourDetailsDto
                {
                    TourType = service.TourService.TourType,
                    DurationHours = service.TourService.DurationHours,
                    DifficultyLevel = service.TourService.DifficultyLevel,
                    MinParticipants = service.TourService.MinParticipants,
                    MaxParticipants = service.TourService.MaxParticipants,
                    Includes = DeserializeList(service.TourService.Includes),
                    Excludes = DeserializeList(service.TourService.Excludes),
                    WhatToBring = service.TourService.WhatToBring,
                    CancellationPolicy = service.TourService.CancellationPolicy,
                    AgeRestrictions = service.TourService.AgeRestrictions,
                    FitnessRequirements = service.TourService.FitnessRequirements
                },
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt
            };
        }

        private static void ValidateServiceRequest(ServiceType serviceType, decimal platformFeeAmount, PartnerTourDetailsDto? tourDetails)
        {
            if (serviceType == ServiceType.Tour && tourDetails == null)
            {
                throw new InvalidOperationException("Tour details are required when creating or updating a tour service.");
            }

            if (serviceType != ServiceType.Tour && tourDetails != null)
            {
                throw new InvalidOperationException("Tour details can only be supplied for tour services.");
            }

            if (serviceType == ServiceType.Other && platformFeeAmount <= 0)
            {
                throw new InvalidOperationException("Other services must include a positive platform fee amount per post.");
            }

            if (serviceType != ServiceType.Other && platformFeeAmount != 0)
            {
                throw new InvalidOperationException("Platform fee amount is only supported for other services.");
            }
        }

        private static TourService MapTour(Guid serviceId, PartnerTourDetailsDto dto)
        {
            return new TourService
            {
                TourId = Guid.NewGuid(),
                ServiceId = serviceId,
                TourType = dto.TourType,
                DurationHours = dto.DurationHours,
                DifficultyLevel = dto.DifficultyLevel,
                MinParticipants = dto.MinParticipants,
                MaxParticipants = dto.MaxParticipants,
                Includes = SerializeList(dto.Includes),
                Excludes = SerializeList(dto.Excludes),
                WhatToBring = dto.WhatToBring ?? string.Empty,
                CancellationPolicy = dto.CancellationPolicy ?? string.Empty,
                AgeRestrictions = dto.AgeRestrictions ?? string.Empty,
                FitnessRequirements = dto.FitnessRequirements ?? string.Empty
            };
        }

        private static void UpdateTour(TourService tour, PartnerTourDetailsDto dto)
        {
            tour.TourType = dto.TourType;
            tour.DurationHours = dto.DurationHours;
            tour.DifficultyLevel = dto.DifficultyLevel;
            tour.MinParticipants = dto.MinParticipants;
            tour.MaxParticipants = dto.MaxParticipants;
            tour.Includes = SerializeList(dto.Includes);
            tour.Excludes = SerializeList(dto.Excludes);
            tour.WhatToBring = dto.WhatToBring ?? string.Empty;
            tour.CancellationPolicy = dto.CancellationPolicy ?? string.Empty;
            tour.AgeRestrictions = dto.AgeRestrictions ?? string.Empty;
            tour.FitnessRequirements = dto.FitnessRequirements ?? string.Empty;
        }

        private static string SerializeList(IEnumerable<string>? values)
        {
            return values == null ? "[]" : JsonSerializer.Serialize(values);
        }

        private static List<string> DeserializeList(string? payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                return new List<string>();
            }

            return JsonSerializer.Deserialize<List<string>>(payload) ?? new List<string>();
        }
    }
}
