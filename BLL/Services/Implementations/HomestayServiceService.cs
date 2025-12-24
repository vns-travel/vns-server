using BLL.Services.Interfaces;
using BLL.DTOs;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Models.Enum;

namespace BLL.Services.Implementations
{
    public class HomestayServiceService : IHomestayService
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomestayServiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ICollection<HomestayService>> GetAllAsync() => await _unitOfWork.HomestayService.GetAllAsync();
        public async Task<HomestayService?> GetByIdAsync(Guid id) => await _unitOfWork.HomestayService.GetAsync(x => x.HomestayId == id);
        public async Task AddAsync(HomestayService entity) { await _unitOfWork.HomestayService.AddAsync(entity); await _unitOfWork.SaveChangesAsync(); }
        public async Task UpdateAsync(HomestayService entity) { await _unitOfWork.HomestayService.UpdateAsync(entity); await _unitOfWork.SaveChangesAsync(); }
        public async Task DeleteAsync(Guid id) { var entity = await _unitOfWork.HomestayService.GetAsync(x => x.HomestayId == id); if (entity != null) { await _unitOfWork.HomestayService.RemoveAsync(entity); await _unitOfWork.SaveChangesAsync(); } }

		public async Task<(Guid homestayId, Guid serviceId, Guid locationId)> CreatePartnerHomestayAsync(Guid partnerId, CreateHomestayRequestDto dto)
		{
			var location = new Location
			{
				LocationId = Guid.NewGuid(),
				Name = dto.Location.Name,
				Address = dto.Location.Address,
				City = dto.Location.City,
				District = dto.Location.District,
				Ward = dto.Location.Ward,
				PostalCode = dto.Location.PostalCode,
				Latitude = dto.Location.Latitude,
				Longitude = dto.Location.Longitude,
				PhoneNumber = dto.Location.PhoneNumber,
				OpeningHours = dto.Location.OpeningHours,
				Description = dto.Description
			};
			await _unitOfWork.Location.AddAsync(location);

			var service = new Service
			{
				ServiceId = Guid.NewGuid(),
				PartnerId = partnerId,
				LocationId = location.LocationId,
				ServiceType = (int)ServiceType.Homestay,
				Title = dto.Title,
				Description = dto.Description,
				IsActive = true
			};
			await _unitOfWork.Service.AddAsync(service);

			var partnerLocation = new PartnerLocation
			{
				PartnerId = partnerId,
				LocationId = location.LocationId,
				IsPrimary = false
			};
			await _unitOfWork.PartnerLocation.AddAsync(partnerLocation);

			var homestay = new HomestayService
			{
				HomestayId = Guid.NewGuid(),
				ServiceId = service.ServiceId,
				CheckInTime = TimeSpan.Parse(dto.CheckInTime),
				CheckOutTime = TimeSpan.Parse(dto.CheckOutTime),
				CancellationPolicy = dto.CancellationPolicy ?? string.Empty,
				HouseRules = dto.HouseRules ?? string.Empty
			};
			await _unitOfWork.HomestayService.AddAsync(homestay);

			await _unitOfWork.SaveChangesAsync();

			return (homestay.HomestayId, service.ServiceId, location.LocationId);
		}

		public async Task<Guid> CreateRoomAsync(Guid homestayId, CreateHomestayRoomRequestDto dto)
		{
			if (dto.NumberOfRooms <= 0) dto.NumberOfRooms = 1;
			// Ensure homestay exists
			var homestay = await _unitOfWork.HomestayService.GetAsync(h => h.HomestayId == homestayId);
			if (homestay == null) throw new KeyNotFoundException("Homestay not found");
			Guid lastRoomId = Guid.Empty;
			for (int i = 0; i < dto.NumberOfRooms; i++)
			{
				var room = new HomestayRoom
				{
					RoomId = Guid.NewGuid(),
					HomestayId = homestayId,
					RoomName = dto.RoomName,
					RoomDescription = dto.RoomDescription ?? string.Empty,
					MaxOccupancy = dto.MaxOccupancy,
					RoomSizeSqm = dto.RoomSizeSqm,
					BedType = dto.BedType ?? string.Empty,
					BedCount = dto.BedCount,
					PrivateBathroom = dto.PrivateBathroom,
					BasePrice = dto.BasePrice,
					WeekendPrice = dto.WeekendPrice,
					HolidayPrice = dto.HolidayPrice,
					RoomAmenities = (dto.RoomAmenities != null && dto.RoomAmenities.Any()) ? System.Text.Json.JsonSerializer.Serialize(dto.RoomAmenities) : string.Empty,
					Images = string.Empty
				};
				await _unitOfWork.HomestayRoom.AddAsync(room);
				lastRoomId = room.RoomId;
			}
			await _unitOfWork.SaveChangesAsync();
			return lastRoomId;
		}

		public async Task<int> CreateAvailabilityBulkAsync(Guid homestayId, BulkAvailabilityRequestDto dto)
		{
			if (dto.EndDate < dto.StartDate) throw new ArgumentException("EndDate must be after StartDate");
			// Ensure homestay exists
			var homestay = await _unitOfWork.HomestayService.GetAsync(h => h.HomestayId == homestayId);
			if (homestay == null) throw new KeyNotFoundException("Homestay not found");
			int generated = 0;
			var totalDays = (dto.EndDate.Date - dto.StartDate.Date).Days + 1;
			for (int d = 0; d < totalDays; d++)
			{
				var date = dto.StartDate.Date.AddDays(d);
				foreach (var room in dto.Rooms)
				{
					var availability = new HomestayAvailability
					{
						AvailabilityId = Guid.NewGuid(),
						HomestayId = homestayId,
						RoomId = room.RoomId,
						Date = date,
						IsAvailable = true,
						Price = room.DefaultPrice,
						MinNights = room.MinNights
					};
					await _unitOfWork.HomestayAvailability.AddAsync(availability);
					generated++;
				}
			}
			await _unitOfWork.SaveChangesAsync();
			return generated;
		}

		public async Task<HomestayActivationResponseDto> ActivateHomestayAsync(Guid homestayId, HomestayActivationRequestDto dto)
		{
			var homestay = await _unitOfWork.HomestayService.GetAsync(h => h.HomestayId == homestayId);
			if (homestay == null) throw new KeyNotFoundException("Homestay not found");
			var service = await _unitOfWork.Service.GetAsync(s => s.ServiceId == homestay.ServiceId);
			if (service == null) throw new KeyNotFoundException("Service not found");

			if (dto.Confirmed)
			{
				service.IsActive = true;
				service.UpdatedAt = DateTime.UtcNow;
				await _unitOfWork.Service.UpdateAsync(service);
				await _unitOfWork.SaveChangesAsync();
			}

			return new HomestayActivationResponseDto
			{
				HomestayId = homestay.HomestayId,
				ServiceId = service.ServiceId,
				Status = service.IsActive ? "Active" : "Inactive"
			};
		}
    }
} 