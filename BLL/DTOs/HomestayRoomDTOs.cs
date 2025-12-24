using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
	public class CreateHomestayRoomRequestDto
	{
		public required string RoomName { get; set; }
		public string? RoomDescription { get; set; }
		public int MaxOccupancy { get; set; }
		public decimal? RoomSizeSqm { get; set; }
		public string? BedType { get; set; }
		public int BedCount { get; set; }
		public bool PrivateBathroom { get; set; }
		public decimal BasePrice { get; set; }
		public decimal WeekendPrice { get; set; }
		public decimal HolidayPrice { get; set; }
		public List<string>? RoomAmenities { get; set; }
		public int NumberOfRooms { get; set; }
	}

	public class CreateHomestayRoomResponseDto
	{
		public Guid RoomId { get; set; }
	}
}


