using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.Enum;
using DAL.Repositories.Interfaces;
using System.Text.Json;

namespace BLL.Services.Implementations
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VoucherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<VoucherDto> CreateAsync(Guid managerUserId, VoucherCreateDto dto)
        {
            ValidateVoucherRequest(dto);

            var manager = await _unitOfWork.User.GetAsync(u => u.UserId == managerUserId);
            if (manager == null)
            {
                throw new KeyNotFoundException("Manager user not found");
            }

            if (await _unitOfWork.Voucher.AnyAsync(v => v.VoucherCode == dto.VoucherCode))
            {
                throw new InvalidOperationException("Voucher code already exists.");
            }

            var voucher = new Voucher
            {
                VoucherId = Guid.NewGuid(),
                UserId = dto.IsPublic ? null : dto.UserId,
                VoucherCode = dto.VoucherCode.Trim().ToUpperInvariant(),
                DiscountPercentage = dto.DiscountPercentage,
                DiscountAmount = dto.DiscountAmount,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                ServiceTypes = JsonSerializer.Serialize(dto.ServiceTypes),
                IsPublic = dto.IsPublic,
                MaxUsage = dto.MaxUsage,
                CurrentUsage = 0,
                CreatedBy = manager.Email
            };

            await _unitOfWork.Voucher.AddAsync(voucher);
            await _unitOfWork.SaveChangesAsync();

            return MapVoucher(voucher);
        }

        public async Task<IEnumerable<VoucherDto>> GetAvailableVouchersAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            var vouchers = await _unitOfWork.Voucher.GetAllAsync(v =>
                (v.IsPublic || v.UserId == userId) &&
                v.ValidFrom <= now &&
                v.ValidTo >= now &&
                v.CurrentUsage < v.MaxUsage);

            return vouchers.Select(MapVoucher);
        }

        public async Task<VoucherValidationResultDto?> ValidateAsync(Guid userId, string voucherCode, decimal originalAmount, IEnumerable<ServiceType> serviceTypes)
        {
            var normalizedCode = voucherCode.Trim().ToUpperInvariant();
            var voucher = await _unitOfWork.Voucher.GetAsync(v => v.VoucherCode == normalizedCode);
            if (voucher == null)
            {
                return null;
            }

            var now = DateTime.UtcNow;
            if (voucher.ValidFrom > now || voucher.ValidTo < now)
            {
                throw new InvalidOperationException("Voucher is not active.");
            }

            if (!voucher.IsPublic && voucher.UserId != userId)
            {
                throw new UnauthorizedAccessException("Voucher does not belong to this user.");
            }

            if (voucher.CurrentUsage >= voucher.MaxUsage)
            {
                throw new InvalidOperationException("Voucher has reached its usage limit.");
            }

            var allowedTypes = DeserializeServiceTypes(voucher.ServiceTypes);
            var bookingTypes = serviceTypes.Distinct().ToList();
            if (allowedTypes.Count > 0 && bookingTypes.Any(type => !allowedTypes.Contains(type)))
            {
                throw new InvalidOperationException("Voucher is not applicable to the selected services.");
            }

            var discountAmount = CalculateDiscount(voucher, originalAmount);
            var finalAmount = Math.Max(0, originalAmount - discountAmount);

            return new VoucherValidationResultDto
            {
                VoucherId = voucher.VoucherId,
                VoucherCode = voucher.VoucherCode,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount
            };
        }

        public async Task IssueWelcomeVoucherAsync(Guid userId)
        {
            var existingVoucher = await _unitOfWork.Voucher.GetAsync(v =>
                v.UserId == userId &&
                v.CreatedBy == "System" &&
                v.DiscountPercentage == 20);

            if (existingVoucher != null)
            {
                return;
            }

            var voucher = new Voucher
            {
                VoucherId = Guid.NewGuid(),
                UserId = userId,
                VoucherCode = $"WELCOME20-{Guid.NewGuid():N}"[..18].ToUpperInvariant(),
                DiscountPercentage = 20,
                DiscountAmount = null,
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddMonths(1),
                ServiceTypes = JsonSerializer.Serialize(new[] { ServiceType.Homestay, ServiceType.Tour }),
                IsPublic = false,
                MaxUsage = 1,
                CurrentUsage = 0,
                CreatedBy = "System"
            };

            await _unitOfWork.Voucher.AddAsync(voucher);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task IncrementUsageAsync(Guid voucherId)
        {
            var voucher = await _unitOfWork.Voucher.GetAsync(v => v.VoucherId == voucherId, tracked: true);
            if (voucher == null)
            {
                throw new KeyNotFoundException("Voucher not found");
            }

            voucher.CurrentUsage += 1;
            await _unitOfWork.Voucher.UpdateAsync(voucher);
            await _unitOfWork.SaveChangesAsync();
        }

        private static void ValidateVoucherRequest(VoucherCreateDto dto)
        {
            if (dto.ValidTo <= dto.ValidFrom)
            {
                throw new InvalidOperationException("Voucher valid-to date must be later than valid-from date.");
            }

            if (dto.MaxUsage <= 0)
            {
                throw new InvalidOperationException("Voucher max usage must be greater than zero.");
            }

            var hasPercentage = dto.DiscountPercentage.HasValue && dto.DiscountPercentage > 0;
            var hasAmount = dto.DiscountAmount.HasValue && dto.DiscountAmount > 0;
            if (hasPercentage == hasAmount)
            {
                throw new InvalidOperationException("Provide exactly one voucher discount type.");
            }

            if (!dto.IsPublic && !dto.UserId.HasValue)
            {
                throw new InvalidOperationException("A non-public voucher must target a specific user.");
            }
        }

        private static decimal CalculateDiscount(Voucher voucher, decimal originalAmount)
        {
            decimal discount;
            if (voucher.DiscountPercentage.HasValue)
            {
                discount = Math.Round(originalAmount * (voucher.DiscountPercentage.Value / 100m), 2);
            }
            else
            {
                discount = voucher.DiscountAmount ?? 0;
            }

            return Math.Min(discount, originalAmount);
        }

        private static VoucherDto MapVoucher(Voucher voucher)
        {
            return new VoucherDto
            {
                VoucherId = voucher.VoucherId,
                UserId = voucher.UserId,
                VoucherCode = voucher.VoucherCode,
                DiscountPercentage = voucher.DiscountPercentage,
                DiscountAmount = voucher.DiscountAmount,
                ValidFrom = voucher.ValidFrom,
                ValidTo = voucher.ValidTo,
                ServiceTypes = DeserializeServiceTypes(voucher.ServiceTypes),
                IsPublic = voucher.IsPublic,
                MaxUsage = voucher.MaxUsage,
                CurrentUsage = voucher.CurrentUsage,
                CreatedBy = voucher.CreatedBy
            };
        }

        private static List<ServiceType> DeserializeServiceTypes(string? payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                return new List<ServiceType>();
            }

            return JsonSerializer.Deserialize<List<ServiceType>>(payload) ?? new List<ServiceType>();
        }
    }
}
