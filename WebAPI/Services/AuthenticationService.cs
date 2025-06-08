using DAL.ExternalSystems;
using WebAPI.Interfaces;

namespace WebAPI.Services
{
    /// <summary>
    /// Authentication service - handles user login validation and role checking
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Generic authentication for any user type
        public async Task<bool> Authenticate(string username, string password)
        {
            try
            {
                Console.WriteLine($"[AuthenticationService] Authenticating user {username}");

                bool isAuthenticated = ActiveDirectory.Auth(username, password);

                if (isAuthenticated)
                {
                    Console.WriteLine($"[AuthenticationService] Authentication successful for {username}");
                    // Check if user has quota in system
                    var userQuota = await _userRepository.GetUserQuota(username);
                    if (userQuota == null)
                    {
                        Console.WriteLine($"[AuthenticationService] Warning: User {username} authenticated but no quota found");
                    }
                }
                else
                {
                    Console.WriteLine($"[AuthenticationService] Authentication failed for {username}");
                }

                return isAuthenticated;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthenticationService] Error during authentication: {ex.Message}");
                return false;
            }
        }

        // Admin-specific authentication with role validation
        public Task<bool> AuthenticateAdmin(string username, string password)
        {
            try
            {
                Console.WriteLine($"[AuthenticationService] Authenticating admin {username}");

                bool isAuthenticated = ActiveDirectory.Auth(username, password);
                bool isAdmin = ActiveDirectory.IsAdmin(username);

                if (isAuthenticated && isAdmin)
                {
                    Console.WriteLine($"[AuthenticationService] Admin authentication successful for {username}");
                    return Task.FromResult(true);
                }
                else
                {
                    Console.WriteLine($"[AuthenticationService] Admin authentication failed for {username} - Auth: {isAuthenticated}, Admin: {isAdmin}");
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthenticationService] Error during admin authentication: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        // Student-specific authentication with role validation
        public Task<bool> AuthenticateStudent(string username, string password)
        {
            try
            {
                Console.WriteLine($"[AuthenticationService] Authenticating student {username}");

                bool isAuthenticated = ActiveDirectory.Auth(username, password);
                bool isStudent = ActiveDirectory.IsStudent(username);

                if (isAuthenticated && isStudent)
                {
                    Console.WriteLine($"[AuthenticationService] Student authentication successful for {username}");
                    return Task.FromResult(true);
                }
                else
                {
                    Console.WriteLine($"[AuthenticationService] Student authentication failed for {username} - Auth: {isAuthenticated}, Student: {isStudent}");
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthenticationService] Error during student authentication: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}