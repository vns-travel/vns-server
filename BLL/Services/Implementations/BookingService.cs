using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Models.Enum;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVoucherService _voucherService;

        public BookingService(IUnitOfWork unitOfWork, IVoucherService voucherService)
        {
            _unitOfWork = unitOfWork;
            _voucherService = voucherService;
        }

        public async Task CreateBookingAsync(BookingDto bookingDto)
        {
            if (bookingDto == null)
            {
                throw new ArgumentNullException(nameof(bookingDto), "Booking data cannot be null");
            }

            var user = await _unitOfWork.User.GetAsync(u => u.UserId == bookingDto.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            VoucherValidationResultDto? voucherResult = null;
            Combo? combo = null;
            Service? singleService = null;
            var bookingServiceTypes = new List<ServiceType>();
            decimal originalAmount;

            if (bookingDto.BookingType == BookingType.Combo)
            {
                if (!bookingDto.ComboId.HasValue)
                {
                    throw new InvalidOperationException("Combo booking requires a combo id.");
                }

                combo = await _unitOfWork.Combo.GetAsync(c => c.ComboId == bookingDto.ComboId.Value, includeProperties: "ComboItems", tracked: true);
                if (combo == null || !combo.IsActive)
                {
                    throw new KeyNotFoundException("Combo not found or not approved.");
                }

                if (combo.CurrentBookings >= combo.MaxBookings)
                {
                    throw new InvalidOperationException("Combo has reached its booking capacity.");
                }

                originalAmount = combo.DiscountedPrice;
                bookingServiceTypes = combo.ComboItems.Select(item => (ServiceType)item.ServiceType).Distinct().ToList();
                combo.CurrentBookings += 1;
                combo.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                if (!bookingDto.ServiceId.HasValue)
                {
                    throw new InvalidOperationException("Single-service booking requires a service id.");
                }

                singleService = await _unitOfWork.Service.GetAsync(s => s.ServiceId == bookingDto.ServiceId.Value);
                if (singleService == null || !singleService.IsActive)
                {
                    throw new KeyNotFoundException("Service not found or not approved.");
                }

                var serviceType = (ServiceType)singleService.ServiceType;
                if (serviceType == ServiceType.Other)
                {
                    throw new InvalidOperationException("Other services are published as news posts and cannot be booked.");
                }

                bookingServiceTypes.Add(serviceType);
                originalAmount = serviceType switch
                {
                    ServiceType.Homestay => await CalculateHomestayAmountAsync(bookingDto),
                    ServiceType.Tour => await CalculateTourAmountAsync(bookingDto),
                    _ => throw new InvalidOperationException("Unsupported service type.")
                };
            }

            if (!string.IsNullOrWhiteSpace(bookingDto.VoucherCode))
            {
                voucherResult = await _voucherService.ValidateAsync(
                    bookingDto.UserId,
                    bookingDto.VoucherCode,
                    originalAmount,
                    bookingServiceTypes);

                if (voucherResult == null)
                {
                    throw new KeyNotFoundException("Voucher not found.");
                }
            }

            var discountAmount = voucherResult?.DiscountAmount ?? 0;
            var finalAmount = voucherResult?.FinalAmount ?? originalAmount;

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                UserId = bookingDto.UserId,
                ComboId = combo?.ComboId,
                VoucherId = voucherResult?.VoucherId,
                VoucherCode = voucherResult?.VoucherCode,
                BookingReference = GenerateBookingReference(),
                BookingType = (int)bookingDto.BookingType,
                BookingStatus = (int)(bookingDto.BookingStatus == 0 ? BookingStatus.Pending : bookingDto.BookingStatus),
                OriginalAmount = originalAmount,
                DiscountAmount = discountAmount,
                TotalAmount = finalAmount,
                FinalAmount = finalAmount,
                DepositAmount = 0,
                RemainingAmount = finalAmount,
                PaymentMethod = (int)bookingDto.PaymentMethod,
                PaymentStatus = (int)(bookingDto.PaymentStatus == 0 ? PaymentStatus.Pending : bookingDto.PaymentStatus),
                NumberOfGuests = ResolveGuestCount(bookingDto),
                SpecialRequests = bookingDto.SpecialRequests ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Booking.AddAsync(booking);

            if (singleService != null)
            {
                await _unitOfWork.BookingItem.AddAsync(new BookingItem
                {
                    BookingItemId = Guid.NewGuid(),
                    BookingId = booking.BookingId,
                    ServiceId = singleService.ServiceId,
                    ServiceDetailId = bookingDto.BookingType == BookingType.Service && bookingDto.HomestayDetail != null
                        ? bookingDto.HomestayDetail.HomestayId
                        : bookingDto.TourDetail?.ScheduleId,
                    UnitPrice = originalAmount,
                    Quantity = 1,
                    StartDate = bookingDto.HomestayDetail?.CheckInDate
                        ?? (bookingDto.TourDetail == null ? null : DateTime.UtcNow),
                    EndDate = bookingDto.HomestayDetail?.CheckOutDate,
                    ItemStatus = (int)(bookingDto.BookingStatus == 0 ? BookingStatus.Pending : bookingDto.BookingStatus),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                if ((ServiceType)singleService.ServiceType == ServiceType.Homestay)
                {
                    await CreateHomestayBookingAsync(booking, bookingDto, originalAmount);
                }
                else if ((ServiceType)singleService.ServiceType == ServiceType.Tour)
                {
                    await CreateTourBookingAsync(booking, bookingDto);
                }
            }

            await _unitOfWork.Payment.AddAsync(new Payment
            {
                PaymentId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                Amount = finalAmount,
                PaymentType = (int)PaymentType.FullPayment,
                PaymentTime = DateTime.UtcNow,
                TransactionId = $"PENDING-{booking.BookingId:N}"[..24].ToUpperInvariant(),
                PaymentMethod = (int)bookingDto.PaymentMethod,
                PaymentStatus = (int)(bookingDto.PaymentStatus == 0 ? PaymentStatus.Pending : bookingDto.PaymentStatus),
                RefundAmount = 0,
                RefundReason = string.Empty
            });

            await _unitOfWork.SaveChangesAsync();

            if (voucherResult != null)
            {
                await _voucherService.IncrementUsageAsync(voucherResult.VoucherId);
            }
        }

        public async Task<bool> DeleteBookingAsync(Guid bookingId)
        {
            if (bookingId == Guid.Empty)
            {
                throw new ArgumentException("Booking ID cannot be empty", nameof(bookingId));
            }
            var booking = await _unitOfWork.Booking.GetAsync(b => b.BookingId == bookingId, includeProperties: "Combo,Voucher", tracked: true);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found");
            }

            var homestayBookings = await _unitOfWork.HomestayBooking.GetAllAsync(h => h.BookingId == bookingId, tracked: true);
            if (homestayBookings.Any())
            {
                foreach (var homestayBooking in homestayBookings)
                {
                    var stayDates = Enumerable.Range(0, homestayBooking.Nights)
                        .Select(offset => homestayBooking.CheckInDate.Date.AddDays(offset))
                        .ToList();

                    var availabilities = await _unitOfWork.HomestayAvailability.GetAllAsync(
                        a => a.HomestayId == homestayBooking.HomestayId &&
                             stayDates.Contains(a.Date.Date) &&
                             a.RoomId == homestayBooking.RoomId,
                        tracked: true);

                    foreach (var availability in availabilities)
                    {
                        availability.IsAvailable = true;
                        await _unitOfWork.HomestayAvailability.UpdateAsync(availability);
                    }
                }
            }

            var tourBookings = await _unitOfWork.TourBooking.GetAllAsync(t => t.BookingId == bookingId, tracked: true);
            if (tourBookings.Any())
            {
                foreach (var tourBooking in tourBookings)
                {
                    var schedule = await _unitOfWork.TourSchedule.GetAsync(s => s.ScheduleId == tourBooking.ScheduleId, tracked: true);
                    if (schedule != null)
                    {
                        schedule.BookedSlots = Math.Max(0, schedule.BookedSlots - tourBooking.Participants);
                        await _unitOfWork.TourSchedule.UpdateAsync(schedule);
                    }
                }
            }

            if (booking.Combo != null)
            {
                booking.Combo.CurrentBookings = Math.Max(0, booking.Combo.CurrentBookings - 1);
                booking.Combo.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Combo.UpdateAsync(booking.Combo);
            }

            if (booking.Voucher != null)
            {
                booking.Voucher.CurrentUsage = Math.Max(0, booking.Voucher.CurrentUsage - 1);
                await _unitOfWork.Voucher.UpdateAsync(booking.Voucher);
            }

            var payments = await _unitOfWork.Payment.GetAllAsync(p => p.BookingId == bookingId, tracked: true);
            if (payments.Any())
            {
                await _unitOfWork.Payment.RemoveRange(payments);
            }

            var bookingItems = await _unitOfWork.BookingItem.GetAllAsync(i => i.BookingId == bookingId, tracked: true);
            if (bookingItems.Any())
            {
                await _unitOfWork.BookingItem.RemoveRange(bookingItems);
            }

            if (homestayBookings.Any())
            {
                await _unitOfWork.HomestayBooking.RemoveRange(homestayBookings);
            }

            if (tourBookings.Any())
            {
                await _unitOfWork.TourBooking.RemoveRange(tourBookings);
            }

            await _unitOfWork.Booking.RemoveAsync(booking);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }
            var bookings = await _unitOfWork.Booking.GetAllAsync(
                b => b.UserId == userId,
                includeProperties: "HomestayBookings,TourBookings,BookingItems,Voucher,Combo");
            return bookings.Select(MapBooking);
        }

        public async Task<BookingDto> GetBookingAsync(Guid bookingId)
        {
            if (bookingId == Guid.Empty)
            {
                throw new ArgumentException("Booking ID cannot be empty", nameof(bookingId));
            }
            var booking = await _unitOfWork.Booking.GetAsync(
                b => b.BookingId == bookingId,
                includeProperties: "HomestayBookings,TourBookings,BookingItems,Voucher,Combo");
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found");
            }
            return MapBooking(booking);
        }

        public async Task UpdateBookingAsync(Guid bookingId)
        {
            if (bookingId == Guid.Empty)
            {
                throw new ArgumentException("Booking ID cannot be empty", nameof(bookingId));
            }
            var booking = await _unitOfWork.Booking.GetAsync(b => b.BookingId == bookingId, tracked: true);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found");
            }

            booking.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Booking.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<decimal> CalculateHomestayAmountAsync(BookingDto bookingDto)
        {
            var detail = bookingDto.HomestayDetail ?? throw new InvalidOperationException("Homestay booking details are required.");
            if (!bookingDto.ServiceId.HasValue)
            {
                throw new InvalidOperationException("Homestay bookings require a service id.");
            }

            if (detail.CheckOutDate.Date <= detail.CheckInDate.Date)
            {
                throw new InvalidOperationException("Check-out date must be later than check-in date.");
            }

            var homestay = await _unitOfWork.HomestayService.GetAsync(h => h.HomestayId == detail.HomestayId, includeProperties: "Service");
            if (homestay == null || homestay.ServiceId != bookingDto.ServiceId.Value)
            {
                throw new KeyNotFoundException("Homestay booking details do not match the selected service.");
            }

            var stayDates = Enumerable.Range(0, (detail.CheckOutDate.Date - detail.CheckInDate.Date).Days)
                .Select(offset => detail.CheckInDate.Date.AddDays(offset))
                .ToList();

            var availabilities = await _unitOfWork.HomestayAvailability.GetAllAsync(
                a => a.HomestayId == detail.HomestayId &&
                     stayDates.Contains(a.Date.Date) &&
                     a.RoomId == detail.RoomId,
                tracked: true);

            if (availabilities.Count != stayDates.Count || availabilities.Any(a => !a.IsAvailable))
            {
                throw new InvalidOperationException("Selected homestay dates are not fully available.");
            }

            var nights = stayDates.Count;
            var accommodationCost = availabilities.Sum(a => a.Price);
            var total = accommodationCost + detail.CleaningFee + detail.ServiceFee;

            foreach (var availability in availabilities)
            {
                availability.IsAvailable = false;
                await _unitOfWork.HomestayAvailability.UpdateAsync(availability);
            }

            return total;
        }

        private async Task CreateHomestayBookingAsync(Booking booking, BookingDto bookingDto, decimal originalAmount)
        {
            var detail = bookingDto.HomestayDetail ?? throw new InvalidOperationException("Homestay booking details are required.");
            var nights = (detail.CheckOutDate.Date - detail.CheckInDate.Date).Days;

            await _unitOfWork.HomestayBooking.AddAsync(new HomestayBooking
            {
                HomestayBookingId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                HomestayId = detail.HomestayId,
                RoomId = detail.RoomId,
                CheckInDate = detail.CheckInDate.Date,
                CheckOutDate = detail.CheckOutDate.Date,
                Nights = nights,
                Adults = detail.Adults,
                Children = detail.Children,
                BookingType = detail.RoomId.HasValue ? 2 : 1,
                RoomRate = originalAmount - detail.CleaningFee - detail.ServiceFee,
                CleaningFee = detail.CleaningFee,
                ServiceFee = detail.ServiceFee,
                TotalAccommodationCost = originalAmount,
                HostApprovalRequired = detail.HostApprovalRequired
            });
        }

        private async Task CreateTourBookingAsync(Booking booking, BookingDto bookingDto)
        {
            var detail = bookingDto.TourDetail ?? throw new InvalidOperationException("Tour booking details are required.");
            var schedule = await _unitOfWork.TourSchedule.GetAsync(s => s.ScheduleId == detail.ScheduleId, includeProperties: "TourService", tracked: true);
            if (schedule == null || !schedule.IsActive)
            {
                throw new KeyNotFoundException("Tour schedule not found or inactive.");
            }

            if (!bookingDto.ServiceId.HasValue || schedule.TourService?.ServiceId != bookingDto.ServiceId.Value)
            {
                throw new InvalidOperationException("Tour schedule does not belong to the selected service.");
            }

            if (detail.Participants <= 0)
            {
                throw new InvalidOperationException("Tour participants must be greater than zero.");
            }

            if (schedule.BookedSlots + detail.Participants > schedule.AvailableSlots)
            {
                throw new InvalidOperationException("Tour schedule does not have enough remaining slots.");
            }

            schedule.BookedSlots += detail.Participants;
            await _unitOfWork.TourSchedule.UpdateAsync(schedule);

            await _unitOfWork.TourBooking.AddAsync(new TourBooking
            {
                TourBookingId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                ScheduleId = detail.ScheduleId,
                Participants = detail.Participants,
                ParticipantDetails = $"{{\"participants\":{detail.Participants}}}",
                PickupLocation = detail.PickupLocation ?? string.Empty,
                PickupTime = detail.PickupTime
            });
        }

        private async Task<decimal> CalculateTourAmountAsync(BookingDto bookingDto)
        {
            var detail = bookingDto.TourDetail ?? throw new InvalidOperationException("Tour booking details are required.");
            if (detail.Participants <= 0)
            {
                throw new InvalidOperationException("Tour participants must be greater than zero.");
            }

            var schedule = await _unitOfWork.TourSchedule.GetAsync(s => s.ScheduleId == detail.ScheduleId, includeProperties: "TourService");
            if (schedule == null || !schedule.IsActive)
            {
                throw new KeyNotFoundException("Tour schedule not found or inactive.");
            }

            if (!bookingDto.ServiceId.HasValue || schedule.TourService?.ServiceId != bookingDto.ServiceId.Value)
            {
                throw new InvalidOperationException("Tour schedule does not belong to the selected service.");
            }

            if (schedule.Price <= 0)
            {
                throw new InvalidOperationException("Tour schedule price is invalid.");
            }

            return schedule.Price * detail.Participants;
        }

        private static int ResolveGuestCount(BookingDto bookingDto)
        {
            if (bookingDto.NumberOfGuests > 0)
            {
                return bookingDto.NumberOfGuests;
            }

            if (bookingDto.HomestayDetail != null)
            {
                return bookingDto.HomestayDetail.Adults + bookingDto.HomestayDetail.Children;
            }

            if (bookingDto.TourDetail != null)
            {
                return bookingDto.TourDetail.Participants;
            }

            return 1;
        }

        private static string GenerateBookingReference()
        {
            return $"BK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..20].ToUpperInvariant();
        }

        private static BookingDto MapBooking(Booking booking)
        {
            var homestay = booking.HomestayBookings?.FirstOrDefault();
            var tour = booking.TourBookings?.FirstOrDefault();
            var serviceItem = booking.BookingItems?.FirstOrDefault();

            return new BookingDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                ServiceId = serviceItem?.ServiceId,
                ComboId = booking.ComboId,
                BookingReference = booking.BookingReference,
                BookingType = (BookingType)booking.BookingType,
                BookingStatus = (BookingStatus)booking.BookingStatus,
                OriginalAmount = booking.OriginalAmount,
                DiscountAmount = booking.DiscountAmount,
                TotalAmount = booking.TotalAmount,
                FinalAmount = booking.FinalAmount,
                PaymentMethod = (PaymentMethod)booking.PaymentMethod,
                PaymentStatus = (PaymentStatus)booking.PaymentStatus,
                NumberOfGuests = booking.NumberOfGuests,
                SpecialRequests = booking.SpecialRequests,
                VoucherCode = booking.VoucherCode,
                HomestayDetail = homestay == null ? null : new BookingHomestayDetailDto
                {
                    HomestayId = homestay.HomestayId,
                    RoomId = homestay.RoomId,
                    CheckInDate = homestay.CheckInDate,
                    CheckOutDate = homestay.CheckOutDate,
                    Adults = homestay.Adults,
                    Children = homestay.Children,
                    CleaningFee = homestay.CleaningFee,
                    ServiceFee = homestay.ServiceFee,
                    HostApprovalRequired = homestay.HostApprovalRequired
                },
                TourDetail = tour == null ? null : new BookingTourDetailDto
                {
                    ScheduleId = tour.ScheduleId,
                    Participants = tour.Participants,
                    PickupLocation = tour.PickupLocation,
                    PickupTime = tour.PickupTime
                }
            };
        }
    }
}
