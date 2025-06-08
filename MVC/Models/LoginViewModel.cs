using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    /// <summary>
    /// View model for user authentication - used by both Faculty and PoS interfaces
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; } // Display authentication errors
    }
}