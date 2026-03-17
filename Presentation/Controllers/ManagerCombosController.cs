using BLL.Services.Interfaces;
using DAL.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/manager/combos")]
    [Authorize]
    public class ManagerCombosController : ControllerBase
    {
        private readonly IComboService _comboService;

        public ManagerCombosController(IComboService comboService)
        {
            _comboService = comboService;
        }

        [HttpPut("{comboId:guid}/approve")]
        public async Task<IActionResult> Approve(Guid comboId)
        {
            try
            {
                User.RequireRole(Role.Manager, Role.SuperAdmin);
                var approved = await _comboService.ApproveComboAsync(comboId);
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
