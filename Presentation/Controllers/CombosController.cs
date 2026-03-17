using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/combos")]
    public class CombosController : ControllerBase
    {
        private readonly IComboService _comboService;

        public CombosController(IComboService comboService)
        {
            _comboService = comboService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveCombos()
        {
            var combos = await _comboService.GetAllCombosAsync();
            return Ok(combos);
        }

        [HttpGet("{comboId:guid}")]
        public async Task<IActionResult> GetById(Guid comboId)
        {
            var combo = await _comboService.GetComboByIdAsync(comboId);
            return combo == null ? NotFound("Combo not found") : Ok(combo);
        }
    }
}
