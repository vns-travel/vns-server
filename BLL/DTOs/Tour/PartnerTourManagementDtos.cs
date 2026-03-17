namespace BLL.DTOs.Tour
{
    public class CreateTourScheduleDto
    {
        public DateTime TourDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int AvailableSlots { get; set; }
        public string? GuideId { get; set; }
        public string? MeetingPoint { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal Price { get; set; }
    }

    public class TourScheduleResponseDto
    {
        public Guid ScheduleId { get; set; }
        public Guid TourId { get; set; }
        public DateTime TourDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int AvailableSlots { get; set; }
        public int BookedSlots { get; set; }
        public string? GuideId { get; set; }
        public string? MeetingPoint { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
    }

    public class TourItineraryResponseDto
    {
        public Guid ItineraryId { get; set; }
        public Guid TourId { get; set; }
        public int StepOrder { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Activity { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
