using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using WebAPI.Interfaces;

namespace WebAPI.Repositories
{
    /// <summary>
    /// Repository for user data operations - handles database interactions
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly PrintSystemContext _context;

        public UserRepository(PrintSystemContext context)
        {
            _context = context;
        }

        // Convert UID (from card scanner) to username
        public async Task<string> GetUsername(string uid)
        {
            try
            {
                Console.WriteLine($"[UserRepository] Looking up username for UID: {uid}");

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Uid == uid);

                if (user == null)
                {
                    Console.WriteLine($"[UserRepository] No user found for UID: {uid}");
                    return string.Empty;
                }

                Console.WriteLine($"[UserRepository] Found user {user.Username} for UID: {uid}");
                return user.Username;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error looking up username: {ex.Message}");
                return string.Empty;
            }
        }

        // Add CHF amount directly to user quota
        public async Task AddAmount(float chfAmount, string username)
        {
            try
            {
                Console.WriteLine($"[UserRepository] Adding {chfAmount} CHF to {username}");

                var userQuota = await _context.UserQuotas
                    .FirstOrDefaultAsync(uq => uq.Username == username);

                if (userQuota == null)
                {
                    Console.WriteLine($"[UserRepository] Creating new quota for {username}");
                    userQuota = new UserQuota
                    {
                        Username = username,
                        TotalPages = 0,
                        LastUpdated = DateTime.Now
                    };
                    _context.UserQuotas.Add(userQuota);
                }

                float pages = (float)(chfAmount / 0.08); // Convert CHF to pages
                userQuota.AddPages(pages);

                await _context.SaveChangesAsync();
                Console.WriteLine($"[UserRepository] Successfully added {chfAmount} CHF ({pages} pages) to {username}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error adding amount: {ex.Message}");
                throw;
            }
        }

        // Add pages to user quota (after CHF conversion)
        public async Task AddPages(float pages, string username)
        {
            try
            {
                Console.WriteLine($"[UserRepository] Adding {pages} pages to {username}");

                var userQuota = await _context.UserQuotas
                    .FirstOrDefaultAsync(uq => uq.Username == username);

                if (userQuota == null)
                {
                    Console.WriteLine($"[UserRepository] Creating new quota for {username}");
                    userQuota = new UserQuota
                    {
                        Username = username,
                        TotalPages = 0,
                        LastUpdated = DateTime.Now
                    };
                    _context.UserQuotas.Add(userQuota);
                }

                userQuota.AddPages(pages);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[UserRepository] Successfully added {pages} pages to {username}. Total: {userQuota.TotalPages}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error adding pages: {ex.Message}");
                throw;
            }
        }

        // Get complete user quota information
        public async Task<UserQuota> GetUserQuota(string username)
        {
            try
            {
                Console.WriteLine($"[UserRepository] Retrieving quota for {username}");

                var userQuota = await _context.UserQuotas
                    .FirstOrDefaultAsync(uq => uq.Username == username);

                if (userQuota == null)
                {
                    Console.WriteLine($"[UserRepository] No quota found for {username}");
                    return null;
                }

                Console.WriteLine($"[UserRepository] Found quota for {username}: {userQuota.TotalPages} pages");
                return userQuota;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error retrieving quota: {ex.Message}");
                return null;
            }
        }

        // Update existing user quota
        public async Task UpdateUserQuota(UserQuota quota)
        {
            try
            {
                Console.WriteLine($"[UserRepository] Updating quota for {quota.Username}");

                var existingQuota = await _context.UserQuotas
                    .FirstOrDefaultAsync(uq => uq.Username == quota.Username);

                if (existingQuota != null)
                {
                    existingQuota.TotalPages = quota.TotalPages;
                    existingQuota.LastUpdated = DateTime.Now;
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"[UserRepository] Successfully updated quota for {quota.Username}");
                }
                else
                {
                    Console.WriteLine($"[UserRepository] Quota not found for update: {quota.Username}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error updating quota: {ex.Message}");
                throw;
            }
        }

        // Get all user balances for admin interface
        public async Task<List<UserQuota>> GetAllUserBalances()
        {
            try
            {
                Console.WriteLine($"[UserRepository] Retrieving all user balances");

                var userQuotas = await _context.UserQuotas
                    .OrderBy(uq => uq.Username)
                    .ToListAsync();

                Console.WriteLine($"[UserRepository] Found {userQuotas.Count} user quotas");
                return userQuotas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error retrieving all balances: {ex.Message}");
                return new List<UserQuota>();
            }
        }
    }
}