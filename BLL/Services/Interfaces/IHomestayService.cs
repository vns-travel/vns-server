using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IHomestayService
    {
        Task<ICollection<HomestayService>> GetAllAsync();
        Task<HomestayService?> GetByIdAsync(Guid id);
        Task AddAsync(HomestayService entity);
        Task UpdateAsync(HomestayService entity);
        Task DeleteAsync(Guid id);
        Task<(Guid homestayId, Guid serviceId, Guid locationId)> CreatePartnerHomestayAsync(Guid partnerId, CreateHomestayRequestDto dto);
        Task<Guid> CreateRoomAsync(Guid partnerId, Guid homestayId, CreateHomestayRoomRequestDto dto);
        Task<int> CreateAvailabilityBulkAsync(Guid partnerId, Guid homestayId, BulkAvailabilityRequestDto dto);
        Task<HomestayActivationResponseDto> ActivateHomestayAsync(Guid partnerId, Guid homestayId, HomestayActivationRequestDto dto);
    }
} 