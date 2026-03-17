using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/manager/vouchers")]
    [Authorize]
    public class ManagerVouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public ManagerVouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VoucherCreateDto dto)
        {
            try
            {
                User.RequireRole(Role.Manager, Role.SuperAdmin);
                var managerUserId = User.GetRequiredUserId();
                var created = await _voucherService.CreateAsync(managerUserId, dto);
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
    }
}
