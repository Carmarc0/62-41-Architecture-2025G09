using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    /// <summary>
    /// Model for adding semester quotas - Faculty interface
    /// </summary>
    public class AddQuotaViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 1000, ErrorMessage = "Amount must be between 1 and 1000 CHF")]
        [Display(Name = "Amount (CHF)")]
        public int Amount { get; set; } // Integer only for CHF amounts

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Model for adding credit - Student PoS interface
    /// </summary>
    public class AddCreditViewModel
    {
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 500, ErrorMessage = "Amount must be between 1 and 500 CHF")]
        [Display(Name = "Amount to add (CHF)")]
        public int Amount { get; set; } // Integer only for CHF amounts

        public double CurrentBalance { get; set; } // Display current balance
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Model for balance checking functionality
    /// </summary>
    public class ViewBalanceViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username or UID")]
        public string Username { get; set; } = string.Empty;

        public double Balance { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Model for displaying user balance information in admin interface
    /// </summary>
    public class UserBalanceInfo
    {
        public string Username { get; set; } = string.Empty;
        public double BalanceChf { get; set; }
        public int Pages { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}