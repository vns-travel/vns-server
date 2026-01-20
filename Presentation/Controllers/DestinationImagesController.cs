using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/destination-images")]
    public class DestinationImagesController : ControllerBase
    {
        private readonly IDestinationImageService _destinationImageService;

        public DestinationImagesController(IDestinationImageService destinationImageService)
        {
            _destinationImageService = destinationImageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByDestination([FromQuery] Guid destinationId)
        {
            if (destinationId == Guid.Empty)
            {
                return BadRequest("destinationId is required");
            }
            var images = await _destinationImageService.GetByDestinationIdAsync(destinationId);
            return Ok(images);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var image = await _destinationImageService.GetByIdAsync(id);
            return image == null ? NotFound() : Ok(image);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DestinationImageDto dto)
        {
            var created = await _destinationImageService.CreateAsync(dto);
            return StatusCode(201, created);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _destinationImageService.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
