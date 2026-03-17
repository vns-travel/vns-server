using DAL.Models.Enum;

namespace BLL.DTOs
{
    public class PartnerTourDetailsDto
    {
        public int TourType { get; set; }
        public int DurationHours { get; set; }
        public int DifficultyLevel { get; set; }
        public int MinParticipants { get; set; }
        public int MaxParticipants { get; set; }
        public List<string>? Includes { get; set; }
        public List<string>? Excludes { get; set; }
        public string? WhatToBring { get; set; }
        public string? CancellationPolicy { get; set; }
        public string? AgeRestrictions { get; set; }
        public string? FitnessRequirements { get; set; }
    }

    public class PartnerServiceCreateDto
    {
        public Guid LocationId { get; set; }
        public Guid? DestinationId { get; set; }
        public ServiceType ServiceType { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public decimal PlatformFeeAmount { get; set; }
        public PartnerTourDetailsDto? TourDetails { get; set; }
    }

    public class PartnerServiceUpdateDto
    {
        public Guid LocationId { get; set; }
        public Guid? DestinationId { get; set; }
        public ServiceType ServiceType { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public decimal PlatformFeeAmount { get; set; }
        public PartnerTourDetailsDto? TourDetails { get; set; }
    }

    public class PartnerServiceResponseDto
    {
        public Guid ServiceId { get; set; }
        public Guid? TourId { get; set; }
        public Guid PartnerId { get; set; }
        public Guid LocationId { get; set; }
        public Guid? DestinationId { get; set; }
        public ServiceType ServiceType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PlatformFeeAmount { get; set; }
        public bool IsActive { get; set; }
        public string PublicationMode { get; set; } = "Service";
        public PartnerTourDetailsDto? TourDetails { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
