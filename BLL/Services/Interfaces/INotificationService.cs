using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetByUserIdAsync(Guid userId);
        Task<NotificationDto?> GetByIdAsync(Guid notificationId);
        Task<NotificationDto> CreateAsync(NotificationDto dto);
        Task<bool> MarkReadAsync(Guid notificationId);
        Task<bool> DeleteAsync(Guid notificationId);
    }
}
