using System;

namespace BLL.DTOs
{
	public class HomestayActivationRequestDto
	{
		public bool Confirmed { get; set; }
	}

	public class HomestayActivationResponseDto
	{
		public Guid HomestayId { get; set; }
		public Guid ServiceId { get; set; }
		public string Status { get; set; } = string.Empty;
	}
}


