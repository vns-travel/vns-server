using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.Enum;
using DAL.Repositories.Interfaces;
using System.Text.Json;

namespace BLL.Services.Implementations
{
    public class ComboService : IComboService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ComboService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ComboDto>> GetAllCombosAsync(bool includeInactive = false)
        {
            var combos = await _unitOfWork.Combo.GetAllAsync(
                c => includeInactive || c.IsActive,
                includeProperties: "ComboItems");

            return combos.Select(MapCombo);
        }

        public async Task<IEnumerable<ComboDto>> GetPartnerCombosAsync(Guid userId)
        {
            var partner = await GetPartnerByUserIdAsync(userId);
            var combos = await _unitOfWork.Combo.GetAllAsync(
                c => c.PartnerId == partner.PartnerId,
                includeProperties: "ComboItems");

            return combos.Select(MapCombo);
        }

        public async Task<ComboDto?> GetComboByIdAsync(Guid comboId, bool includeInactive = false)
        {
            var combo = await _unitOfWork.Combo.GetAsync(
                c => c.ComboId == comboId && (includeInactive || c.IsActive),
                includeProperties: "ComboItems");

            return combo == null ? null : MapCombo(combo);
        }

        public async Task<ComboDto> CreateComboAsync(Guid userId, ComboDto comboDto)
        {
            var partner = await GetPartnerByUserIdAsync(userId);
            ValidateComboRequest(comboDto);

            var serviceIds = comboDto.ComboServices.Select(c => c.ServiceId).ToList();
            if (serviceIds.Count != serviceIds.Distinct().Count())
            {
                throw new InvalidOperationException("A combo cannot contain duplicated service entries.");
            }

            var services = await _unitOfWork.Service.GetAllAsync(
                s => serviceIds.Contains(s.ServiceId),
                tracked: false);

            if (services.Count != serviceIds.Count)
            {
                throw new KeyNotFoundException("One or more services were not found.");
            }

            foreach (var service in services)
            {
                if (service.PartnerId != partner.PartnerId)
                {
                    throw new UnauthorizedAccessException("All services in a combo must belong to the requesting partner.");
                }

                if (!service.IsActive)
                {
                    throw new InvalidOperationException("Only approved services can be added to a combo.");
                }

                if (service.ServiceType != (int)ServiceType.Homestay && service.ServiceType != (int)ServiceType.Tour)
                {
                    throw new InvalidOperationException("Only homestay and tour services can be added as concrete combo items.");
                }
            }

            var combo = new Combo
            {
                ComboId = Guid.NewGuid(),
                PartnerId = partner.PartnerId,
                Title = comboDto.Title,
                Description = comboDto.Description,
                OriginalPrice = comboDto.OriginalPrice,
                DiscountedPrice = comboDto.DiscountedPrice,
                ValidFrom = comboDto.ValidFrom,
                ValidTo = comboDto.ValidTo,
                MaxBookings = comboDto.MaxBookings,
                CurrentBookings = 0,
                IsActive = false,
                AdditionalServices = JsonSerializer.Serialize(comboDto.AdditionalServices),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Combo.AddAsync(combo);

            var comboItems = comboDto.ComboServices.Select(item =>
            {
                var service = services.First(s => s.ServiceId == item.ServiceId);
                return new ComboItem
                {
                    ComboItemId = Guid.NewGuid(),
                    ComboId = combo.ComboId,
                    ServiceId = item.ServiceId,
                    ServiceType = service.ServiceType,
                    Quantity = item.Quantity <= 0 ? 1 : item.Quantity,
                    IncludedFeatures = item.IncludedFeatures ?? string.Empty,
                    SequenceOrder = item.SequenceOrder
                };
            }).ToList();

            await _unitOfWork.ComboItem.AddRange(comboItems);
            await _unitOfWork.SaveChangesAsync();

            return (await GetComboByIdAsync(combo.ComboId, includeInactive: true))!;
        }

        public async Task<ComboDto> ApproveComboAsync(Guid comboId)
        {
            var combo = await _unitOfWork.Combo.GetAsync(c => c.ComboId == comboId, tracked: true);
            if (combo == null)
            {
                throw new KeyNotFoundException("Combo not found");
            }

            combo.IsActive = true;
            combo.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Combo.UpdateAsync(combo);
            await _unitOfWork.SaveChangesAsync();

            return (await GetComboByIdAsync(combo.ComboId, includeInactive: true))!;
        }

        public async Task<bool> DeleteComboAsync(Guid userId, Guid comboId)
        {
            var partner = await GetPartnerByUserIdAsync(userId);
            var combo = await _unitOfWork.Combo.GetAsync(c => c.ComboId == comboId, includeProperties: "ComboItems", tracked: true);
            if (combo == null)
            {
                return false;
            }

            if (combo.PartnerId != partner.PartnerId)
            {
                throw new UnauthorizedAccessException("Combo does not belong to partner");
            }

            if (combo.ComboItems?.Any() == true)
            {
                await _unitOfWork.ComboItem.RemoveRange(combo.ComboItems);
            }

            await _unitOfWork.Combo.RemoveAsync(combo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async Task<Partner> GetPartnerByUserIdAsync(Guid userId)
        {
            var partner = await _unitOfWork.Partner.GetAsync(p => p.UserId == userId);
            if (partner == null)
            {
                throw new KeyNotFoundException("Partner not found");
            }

            return partner;
        }

        private static void ValidateComboRequest(ComboDto comboDto)
        {
            if (comboDto.ComboServices.Count == 0)
            {
                throw new InvalidOperationException("A combo must include at least one homestay or tour service.");
            }

            if (comboDto.ValidTo <= comboDto.ValidFrom)
            {
                throw new InvalidOperationException("Combo valid-to date must be later than valid-from date.");
            }

            if (comboDto.MaxBookings <= 0)
            {
                throw new InvalidOperationException("Combo max bookings must be greater than zero.");
            }

            if (comboDto.OriginalPrice <= 0 || comboDto.DiscountedPrice <= 0)
            {
                throw new InvalidOperationException("Combo pricing must be greater than zero.");
            }

            if (comboDto.DiscountedPrice > comboDto.OriginalPrice)
            {
                throw new InvalidOperationException("Combo discounted price cannot exceed original price.");
            }
        }

        private static ComboDto MapCombo(Combo combo)
        {
            return new ComboDto
            {
                ComboId = combo.ComboId,
                PartnerId = combo.PartnerId,
                Title = combo.Title,
                Description = combo.Description,
                OriginalPrice = combo.OriginalPrice,
                DiscountedPrice = combo.DiscountedPrice,
                ValidFrom = combo.ValidFrom,
                ValidTo = combo.ValidTo,
                MaxBookings = combo.MaxBookings,
                CurrentBookings = combo.CurrentBookings,
                IsActive = combo.IsActive,
                ComboServices = combo.ComboItems?.OrderBy(c => c.SequenceOrder).Select(item => new ComboServiceDto
                {
                    ServiceId = item.ServiceId,
                    Quantity = item.Quantity,
                    SequenceOrder = item.SequenceOrder,
                    IncludedFeatures = item.IncludedFeatures
                }).ToList() ?? new List<ComboServiceDto>(),
                AdditionalServices = DeserializeAdditionalServices(combo.AdditionalServices)
            };
        }

        private static List<ComboAdditionalServiceDto> DeserializeAdditionalServices(string? payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                return new List<ComboAdditionalServiceDto>();
            }

            return JsonSerializer.Deserialize<List<ComboAdditionalServiceDto>>(payload) ?? new List<ComboAdditionalServiceDto>();
        }
    }
}
