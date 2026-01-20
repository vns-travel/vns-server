using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    /// <summary>
    /// External authentication provider link (e.g., Google, Facebook).
    /// </summary>
    public class AuthProvider
    {
        [Key]
        public Guid AuthProviderId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Provider { get; set; } = string.Empty; // e.g., google, facebook

        [Required]
        [StringLength(255)]
        public string ProviderUserId { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AccessToken { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
