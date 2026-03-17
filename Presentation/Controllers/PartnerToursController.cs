using BLL.DTOs.Tour;
using BLL.Services.Interfaces;
using DAL.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/partner/tours")]
    [Authorize]
    public class PartnerToursController : ControllerBase
    {
        private readonly ITourService _tourService;

        public PartnerToursController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpPost("{tourId:guid}/schedules")]
        public async Task<IActionResult> CreateSchedule(Guid tourId, [FromBody] CreateTourScheduleDto dto)
        {
            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
                var created = await _tourService.CreateScheduleAsync(userId, tourId, dto);
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

        [HttpPost("{tourId:guid}/itineraries")]
        public async Task<IActionResult> CreateItinerary(Guid tourId, [FromBody] CreateTourItineraryDto dto)
        {
            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
                dto.TourId = tourId;
                var created = await _tourService.CreateItineraryAsync(userId, tourId, dto);
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
