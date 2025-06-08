using WebAPI.Controllers;

namespace WebAPI.Interfaces
{
    /// <summary>
    /// Service interface for quota management operations
    /// Based on your architecture diagram
    /// </summary>
    public interface IQuotaService
    {
        /// <summary>
        /// Adds credit to a user's print account by converting CHF into pages
        /// and updating the print system accordingly
        /// </summary>
        /// <param name="chfAmount">The amount in Swiss Francs to be converted and added</param>
        /// <param name="username">The user's login ID</param>
        /// <returns>true if the update is successful, false otherwise</returns>
        Task<bool> AddAmount(float chfAmount, string username);

        /// <summary>
        /// Retrieves the current balance of the user's print quota, expressed in CHF
        /// </summary>
        /// <param name="username">The user's login ID</param>
        /// <returns>The CHF value of the available print quota</returns>
        Task<double> GetAvailableAmount(string username);

        /// <summary>
        /// Converts CHF to pages and transfers the equivalent amount to the external print system
        /// </summary>
        /// <param name="username">The user's login ID</param>
        /// <param name="chfAmount">The amount in CHF to transfer</param>
        /// <returns>true if the transfer is successful</returns>
        Task<bool> TransferMoneyToPrintSystem(string username, double chfAmount);

        /// <summary>
        /// Adds a fixed quota for the semester to a user's account
        /// Typically used by faculty-side web interface
        /// </summary>
        /// <param name="username">The user's login ID</param>
        /// <param name="chfAmount">The semester grant (in CHF)</param>
        /// <returns>true if the operation completes successfully</returns>
        Task<bool> AddSemesterQuota(string username, double chfAmount);

        /// <summary>
        /// Converts a UID (e.g., from card scanner) to a corresponding username
        /// </summary>
        /// <param name="uid">The card UID</param>
        /// <returns>The corresponding username</returns>
        Task<string> GetUsername(string uid);

        /// <summary>
        /// Gets all users balances for admin interface
        /// </summary>
        /// <returns>List of user balance information</returns>
        Task<List<UserBalanceResponse>> GetAllBalancesAsync();
    }
}