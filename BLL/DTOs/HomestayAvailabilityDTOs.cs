using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
	public class BulkAvailabilityRequestDto
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public List<BulkAvailabilityRoomDto> Rooms { get; set; } = new();
		public bool ApplyToAllDates { get; set; }
	}

	public class BulkAvailabilityRoomDto
	{
		public Guid RoomId { get; set; }
		public decimal DefaultPrice { get; set; }
		public int MinNights { get; set; }
	}

	public class BulkAvailabilityResponseDto
	{
		public int GeneratedRecords { get; set; }
	}
}


