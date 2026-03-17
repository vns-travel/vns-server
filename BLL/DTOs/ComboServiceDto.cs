namespace BLL.DTOs
{
    public class ComboServiceDto
    {
        public Guid ServiceId { get; set; }
        public int Quantity { get; set; } = 1;
        public int SequenceOrder { get; set; }
        public string? IncludedFeatures { get; set; }
    }
}
