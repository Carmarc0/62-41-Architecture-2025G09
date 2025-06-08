using DAL.ExternalSystems;
using DAL.Utilities;
using WebAPI.Interfaces;
using WebAPI.Controllers;

namespace WebAPI.Services
{
    /// <summary>
    /// Core business service - handles CHF to pages conversion and print system sync
    /// </summary>
    public class QuotaService : IQuotaService
    {
        private readonly IUserRepository _userRepository;

        public QuotaService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Main business flow: CHF → Pages → Update DB → Sync Print System
        public async Task<bool> AddAmount(float chfAmount, string username)
        {
            try
            {
                Console.WriteLine($"[QuotaService] Adding {chfAmount} CHF to user {username}");

                // Validate user exists in Active Directory
                if (!ActiveDirectory.UserExists(username))
                {
                    Console.WriteLine($"[QuotaService] User {username} does not exist in Active Directory");
                    return false;
                }

                // Convert CHF to pages using business rate
                float pages = CurrencyConverter.ConvertCHFToPages(chfAmount);
                Console.WriteLine($"[QuotaService] Converted {chfAmount} CHF to {pages} pages");

                // Update internal database
                await _userRepository.AddPages(pages, username);

                // Sync with external print system
                bool transferSuccess = PrintSystemAPI.AddPages(pages, username);

                if (!transferSuccess)
                {
                    Console.WriteLine($"[QuotaService] Failed to transfer to print system for {username}");
                    return false;
                }

                Console.WriteLine($"[QuotaService] Successfully added {chfAmount} CHF ({pages} pages) to {username}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuotaService] Error adding amount: {ex.Message}");
                return false;
            }
        }

        // Get user balance in CHF for PoS interface
        public async Task<double> GetAvailableAmount(string username)
        {
            try
            {
                var userQuota = await _userRepository.GetUserQuota(username);
                if (userQuota == null)
                {
                    Console.WriteLine($"[QuotaService] User quota not found for {username}");
                    return 0.0;
                }

                double chfBalance = userQuota.GetBalanceInCHF();
                Console.WriteLine($"[QuotaService] Available amount for {username}: {chfBalance} CHF");
                return chfBalance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuotaService] Error getting available amount: {ex.Message}");
                return 0.0;
            }
        }

        // Get all balances for admin interface
        public async Task<List<UserBalanceResponse>> GetAllBalancesAsync()
        {
            try
            {
                Console.WriteLine($"[QuotaService] Getting all user balances");
                var balances = await _userRepository.GetAllUserBalances();

                var result = balances.Select(b => new UserBalanceResponse
                {
                    Username = b.Username,
                    BalanceChf = b.GetBalanceInCHF(),
                    Pages = (int)b.GetPages(),
                    LastUpdated = b.LastUpdated
                }).ToList();

                Console.WriteLine($"[QuotaService] Retrieved {result.Count} user balances");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuotaService] Error getting all balances: {ex.Message}");
                return new List<UserBalanceResponse>();
            }
        }

        // Direct transfer to print system
        public async Task<bool> TransferMoneyToPrintSystem(string username, double chfAmount)
        {
            try
            {
                Console.WriteLine($"[QuotaService] Transferring {chfAmount} CHF to print system for {username}");

                float pages = CurrencyConverter.ConvertCHFToPages(chfAmount);
                bool success = PrintSystemAPI.AddPages(pages, username);

                if (success)
                {
                    Console.WriteLine($"[QuotaService] Successfully transferred {chfAmount} CHF ({pages} pages) to print system");
                }

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuotaService] Error transferring to print system: {ex.Message}");
                return false;
            }
        }

        // Add semester quota (Faculty interface)
        public async Task<bool> AddSemesterQuota(string username, double chfAmount)
        {
            try
            {
                Console.WriteLine($"[QuotaService] Adding semester quota {chfAmount} CHF to {username}");

                // Validate user exists
                if (!ActiveDirectory.UserExists(username))
                {
                    Console.WriteLine($"[QuotaService] User {username} does not exist in Active Directory");
                    return false;
                }

                float pages = CurrencyConverter.ConvertCHFToPages(chfAmount);
                await _userRepository.AddPages(pages, username);

                bool transferSuccess = PrintSystemAPI.AddPages(pages, username);

                Console.WriteLine($"[QuotaService] Semester quota added successfully for {username}");
                return transferSuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuotaService] Error adding semester quota: {ex.Message}");
                return false;
            }
        }

        // Convert UID to username (card scanner support)
        public async Task<string> GetUsername(string uid)
        {
            try
            {
                Console.WriteLine($"[QuotaService] Converting UID {uid} to username");
                string username = await _userRepository.GetUsername(uid);
                Console.WriteLine($"[QuotaService] UID {uid} corresponds to username {username}");
                return username;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuotaService] Error converting UID to username: {ex.Message}");
                return string.Empty;
            }
        }
    }
}