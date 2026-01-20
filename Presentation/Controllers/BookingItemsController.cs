using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/booking-items")]
    public class BookingItemsController : ControllerBase
    {
        private readonly IBookingItemService _bookingItemService;

        public BookingItemsController(IBookingItemService bookingItemService)
        {
            _bookingItemService = bookingItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByBooking([FromQuery] Guid bookingId)
        {
            if (bookingId == Guid.Empty)
            {
                return BadRequest("bookingId is required");
            }
            var items = await _bookingItemService.GetByBookingIdAsync(bookingId);
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _bookingItemService.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingItemDto dto)
        {
            var created = await _bookingItemService.CreateAsync(dto);
            return StatusCode(201, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookingItemDto dto)
        {
            var updated = await _bookingItemService.UpdateAsync(id, dto);
            return updated ? Ok() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _bookingItemService.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
