using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/destinations")]
    public class DestinationsController : ControllerBase
    {
        private readonly IDestinationService _destinationService;

        public DestinationsController(IDestinationService destinationService)
        {
            _destinationService = destinationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var destinations = await _destinationService.GetAllAsync();
            return Ok(destinations);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var destination = await _destinationService.GetByIdAsync(id);
            return destination == null ? NotFound() : Ok(destination);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DestinationDto dto)
        {
            var created = await _destinationService.CreateAsync(dto);
            return StatusCode(201, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DestinationDto dto)
        {
            var updated = await _destinationService.UpdateAsync(id, dto);
            return updated ? Ok() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _destinationService.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
