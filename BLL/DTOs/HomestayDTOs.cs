using System;

namespace BLL.DTOs
{
	public class CreateHomestayRequestDto
	{
		public required string Title { get; set; }
		public required string Description { get; set; }
		public required LocationDto Location { get; set; }
		public required string CheckInTime { get; set; }
		public required string CheckOutTime { get; set; }
		public string? CancellationPolicy { get; set; }
		public string? HouseRules { get; set; }
	}

	public class LocationDto
	{
		public required string Name { get; set; }
		public required string Address { get; set; }
		public string? City { get; set; }
		public string? District { get; set; }
		public string? Ward { get; set; }
		public string? PostalCode { get; set; }
		public decimal? Latitude { get; set; }
		public decimal? Longitude { get; set; }
		public string? PhoneNumber { get; set; }
		public string? OpeningHours { get; set; }
	}

	public class CreateHomestayResponseDto
	{
		public Guid HomestayId { get; set; }
		public Guid ServiceId { get; set; }
		public Guid LocationId { get; set; }
	}
}

