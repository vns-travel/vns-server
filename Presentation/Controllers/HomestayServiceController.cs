using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAL.Repositories.Interfaces;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/partner/homestays")]
    [Authorize]
    public class HomestayServiceController : ControllerBase
    {
        private readonly IHomestayService _homestayService;
        private readonly IUnitOfWork _unitOfWork;

        public HomestayServiceController(IHomestayService homestayService, IUnitOfWork unitOfWork)
        {
            _homestayService = homestayService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHomestay([FromBody] CreateHomestayRequestDto request)
        {
            if (request == null) return BadRequest("Request is null");

            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
                var partner = await _unitOfWork.Partner.GetAsync(p => p.UserId == userId);
                if (partner == null) return Forbid();
                var partnerId = partner.PartnerId;

                var (homestayId, serviceId, locationId) = await _homestayService.CreatePartnerHomestayAsync(partnerId, request);

                var response = new CreateHomestayResponseDto
                {
                    HomestayId = homestayId,
                    ServiceId = serviceId,
                    LocationId = locationId
                };

                return StatusCode(201, response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("{homestayId}/rooms")]
        public async Task<IActionResult> CreateRoom(Guid homestayId, [FromBody] CreateHomestayRoomRequestDto request)
        {
            if (request == null) return BadRequest("Request is null");

            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
                var partner = await _unitOfWork.Partner.GetAsync(p => p.UserId == userId);
                if (partner == null) return Forbid();

                var roomId = await _homestayService.CreateRoomAsync(partner.PartnerId, homestayId, request);
                return StatusCode(201, new CreateHomestayRoomResponseDto { RoomId = roomId });
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

        [HttpPost("{homestayId}/availability/bulk")]
        public async Task<IActionResult> CreateAvailabilityBulk(Guid homestayId, [FromBody] BulkAvailabilityRequestDto request)
        {
            if (request == null) return BadRequest("Request is null");

            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
                var partner = await _unitOfWork.Partner.GetAsync(p => p.UserId == userId);
                if (partner == null) return Forbid();

                var generated = await _homestayService.CreateAvailabilityBulkAsync(partner.PartnerId, homestayId, request);
                return StatusCode(201, new BulkAvailabilityResponseDto { GeneratedRecords = generated });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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

        [HttpPost("{homestayId}/create")]
        public async Task<IActionResult> ActivateHomestay(Guid homestayId, [FromBody] HomestayActivationRequestDto request)
        {
            if (request == null) return BadRequest("Request is null");

            try
            {
                User.RequireRole(Role.Partner);
                var userId = User.GetRequiredUserId();
                var partner = await _unitOfWork.Partner.GetAsync(p => p.UserId == userId);
                if (partner == null) return Forbid();

                var response = await _homestayService.ActivateHomestayAsync(partner.PartnerId, homestayId, request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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


