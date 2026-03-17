using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/vouchers")]
    [Authorize]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyVouchers()
        {
            try
            {
                User.RequireRole(Role.Customer, Role.Partner, Role.Manager, Role.SuperAdmin);
                var userId = User.GetRequiredUserId();
                var vouchers = await _voucherService.GetAvailableVouchersAsync(userId);
                return Ok(vouchers);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] VoucherValidationRequestDto dto)
        {
            try
            {
                User.RequireRole(Role.Customer, Role.Partner, Role.Manager, Role.SuperAdmin);
                var userId = User.GetRequiredUserId();
                var result = await _voucherService.ValidateAsync(userId, dto.VoucherCode, dto.OriginalAmount, dto.ServiceTypes);
                return result == null ? NotFound("Voucher not found") : Ok(result);
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
