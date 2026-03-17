using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/partner/combos")]
    [Authorize]
    public class PartnerCombosController : ControllerBase
    {
        private readonly IComboService _comboService;

        public PartnerCombosController(IComboService comboService)
        {
            _comboService = comboService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
                var combos = await _comboService.GetPartnerCombosAsync(userId);
                return Ok(combos);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ComboDto comboDto)
        {
            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
                var created = await _comboService.CreateComboAsync(userId, comboDto);
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

        [HttpDelete("{comboId:guid}")]
        public async Task<IActionResult> Delete(Guid comboId)
        {
            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
                var deleted = await _comboService.DeleteComboAsync(userId, comboId);
                return deleted ? Ok("Combo deleted successfully") : NotFound("Combo not found");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
