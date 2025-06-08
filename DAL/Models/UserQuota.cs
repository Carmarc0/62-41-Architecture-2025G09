using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    /// <summary>
    /// User print quota entity - tracks pages and CHF conversion
    /// </summary>
    public class UserQuota
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        public float TotalPages { get; set; } // Available pages for printing

        public DateTime LastUpdated { get; set; }

        // Add pages to quota and update timestamp
        public void AddPages(float pages)
        {
            TotalPages += pages;
            LastUpdated = DateTime.Now;
        }

        // Convert pages to CHF (1 page = 0.08 CHF)
        public double GetBalanceInCHF()
        {
            return TotalPages * 0.08;
        }

        // Get current page balance
        public float GetPages()
        {
            return TotalPages;
        }
    }
}