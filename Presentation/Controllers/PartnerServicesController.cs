using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/partner/services")]
    [Authorize]
    public class PartnerServicesController : ControllerBase
    {
        private readonly IPartnerService _partnerService;

        public PartnerServicesController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PartnerServiceCreateDto dto)
        {
            var userId = GetUserId();
            try
            {
                var created = await _partnerService.CreateAsync(userId, dto);
                return StatusCode(201, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{serviceId:guid}")]
        public async Task<IActionResult> Update(Guid serviceId, [FromBody] PartnerServiceUpdateDto dto)
        {
            var userId = GetUserId();
            try
            {
                var updated = await _partnerService.UpdateAsync(userId, serviceId, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpDelete("{serviceId:guid}")]
        public async Task<IActionResult> Delete(Guid serviceId)
        {
            var userId = GetUserId();
            try
            {
                var deleted = await _partnerService.DeleteAsync(userId, serviceId);
                return deleted ? Ok("Service deleted successfully") : NotFound("Service not found");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }
            return Guid.Parse(userIdClaim);
        }
    }
}
