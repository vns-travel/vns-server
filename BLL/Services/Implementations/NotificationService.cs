using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(Guid userId)
        {
            var notifications = await _unitOfWork.Notification.GetAllAsync(n => n.UserId == userId);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<NotificationDto?> GetByIdAsync(Guid notificationId)
        {
            var notification = await _unitOfWork.Notification.GetAsync(n => n.NotificationId == notificationId);
            return notification == null ? null : _mapper.Map<NotificationDto>(notification);
        }

        public async Task<NotificationDto> CreateAsync(NotificationDto dto)
        {
            var entity = _mapper.Map<Notification>(dto);
            entity.NotificationId = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Notification.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<NotificationDto>(entity);
        }

        public async Task<bool> MarkReadAsync(Guid notificationId)
        {
            var entity = await _unitOfWork.Notification.GetAsync(n => n.NotificationId == notificationId);
            if (entity == null)
            {
                return false;
            }

            entity.IsRead = true;
            entity.ReadAt = DateTime.UtcNow;
            await _unitOfWork.Notification.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid notificationId)
        {
            var entity = await _unitOfWork.Notification.GetAsync(n => n.NotificationId == notificationId);
            if (entity == null)
            {
                return false;
            }

            await _unitOfWork.Notification.RemoveAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
