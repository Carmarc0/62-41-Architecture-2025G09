namespace MVC.Models
{
    /// <summary>
    /// Standard error view model for exception handling
    /// </summary>
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // Display request ID only if available
    }
}