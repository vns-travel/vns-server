using BLL.Services.Interfaces;
using DAL.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/manager/services")]
    [Authorize]
    public class ManagerServicesController : ControllerBase
    {
        private readonly IPartnerService _partnerService;

        public ManagerServicesController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        [HttpPut("{serviceId:guid}/approve")]
        public async Task<IActionResult> Approve(Guid serviceId)
        {
            try
            {
                User.RequireRole(Role.Manager, Role.SuperAdmin);
                var approved = await _partnerService.ApproveAsync(serviceId);
                return Ok(approved);
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
