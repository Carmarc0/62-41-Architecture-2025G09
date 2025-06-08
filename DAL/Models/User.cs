using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Uid { get; set; } = string.Empty; // Card UID for scanner

        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        // Navigation property to UserQuota
        public virtual UserQuota? UserQuota { get; set; }
    }
}