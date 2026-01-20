using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByUser([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("userId is required");
            }
            var notifications = await _notificationService.GetByUserIdAsync(userId);
            return Ok(notifications);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var notification = await _notificationService.GetByIdAsync(id);
            return notification == null ? NotFound() : Ok(notification);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationDto dto)
        {
            var created = await _notificationService.CreateAsync(dto);
            return StatusCode(201, created);
        }

        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkRead(Guid id)
        {
            var updated = await _notificationService.MarkReadAsync(id);
            return updated ? Ok() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _notificationService.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
