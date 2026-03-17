using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

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
            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
                var created = await _partnerService.CreateAsync(userId, dto);
                return StatusCode(201, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPut("{serviceId:guid}")]
        public async Task<IActionResult> Update(Guid serviceId, [FromBody] PartnerServiceUpdateDto dto)
        {
            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
                var updated = await _partnerService.UpdateAsync(userId, serviceId, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpDelete("{serviceId:guid}")]
        public async Task<IActionResult> Delete(Guid serviceId)
        {
            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
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
    }
}
