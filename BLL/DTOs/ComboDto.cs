namespace BLL.DTOs
{
    public class ComboAdditionalServiceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ComboDto
    {
        public Guid ComboId { get; set; }
        public Guid PartnerId { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int MaxBookings { get; set; }
        public int CurrentBookings { get; set; }
        public bool IsActive { get; set; }
        public List<ComboServiceDto> ComboServices { get; set; } = new();
        public List<ComboAdditionalServiceDto> AdditionalServices { get; set; } = new();
    }
}
