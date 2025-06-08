using DAL.Models;

namespace WebAPI.Interfaces
{
    /// <summary>
    /// Repository interface for user data operations
    /// Based on your architecture diagram
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Finds a user by UID (typically scanned from a physical ID)
        /// </summary>
        /// <param name="uid">Unique ID (e.g., card code)</param>
        /// <returns>Username</returns>
        Task<string> GetUsername(string uid);

        /// <summary>
        /// Updates a user's balance by adding the given CHF amount (without converting to pages)
        /// </summary>
        /// <param name="chfAmount">Amount in CHF</param>
        /// <param name="username">Target user</param>
        Task AddAmount(float chfAmount, string username);

        /// <summary>
        /// Adds pages to a user's account (used after conversion or system sync)
        /// </summary>
        /// <param name="pages">Number of pages to add</param>
        /// <param name="username">Target user</param>
        Task AddPages(float pages, string username);

        /// <summary>
        /// Retrieves the complete quota object for a user
        /// </summary>
        /// <param name="username">Target user</param>
        /// <returns>A UserQuota entity with balance and history</returns>
        Task<UserQuota> GetUserQuota(string username);

        /// <summary>
        /// Persists changes made to the UserQuota entity in the database
        /// </summary>
        /// <param name="quota">Modified UserQuota entity</param>
        Task UpdateUserQuota(UserQuota quota);

        /// <summary>
        /// Gets all user quotas for admin interface
        /// </summary>
        /// <returns>List of all user quotas</returns>
        Task<List<UserQuota>> GetAllUserBalances();
    }
}