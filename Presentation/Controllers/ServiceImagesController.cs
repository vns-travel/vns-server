using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/service-images")]
    public class ServiceImagesController : ControllerBase
    {
        private readonly IServiceImageService _serviceImageService;

        public ServiceImagesController(IServiceImageService serviceImageService)
        {
            _serviceImageService = serviceImageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByService([FromQuery] Guid serviceId)
        {
            if (serviceId == Guid.Empty)
            {
                return BadRequest("serviceId is required");
            }
            var images = await _serviceImageService.GetByServiceIdAsync(serviceId);
            return Ok(images);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var image = await _serviceImageService.GetByIdAsync(id);
            return image == null ? NotFound() : Ok(image);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceImageDto dto)
        {
            var created = await _serviceImageService.CreateAsync(dto);
            return StatusCode(201, created);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _serviceImageService.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
