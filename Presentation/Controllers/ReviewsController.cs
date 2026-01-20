using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Guid? serviceId, [FromQuery] Guid? bookingId)
        {
            if (serviceId.HasValue && serviceId.Value != Guid.Empty)
            {
                var byService = await _reviewService.GetByServiceIdAsync(serviceId.Value);
                return Ok(byService);
            }

            if (bookingId.HasValue && bookingId.Value != Guid.Empty)
            {
                var byBooking = await _reviewService.GetByBookingIdAsync(bookingId.Value);
                return Ok(byBooking);
            }

            var all = await _reviewService.GetAllAsync();
            return Ok(all);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            return review == null ? NotFound() : Ok(review);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReviewDto dto)
        {
            var created = await _reviewService.CreateAsync(dto);
            return StatusCode(201, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReviewDto dto)
        {
            var updated = await _reviewService.UpdateAsync(id, dto);
            return updated ? Ok() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _reviewService.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
