using Microsoft.AspNetCore.Mvc;
using DAL.ExternalSystems;
using WebAPI.Interfaces;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get all users with their quota information - used by admin interface
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var allUsernames = ActiveDirectory.GetAllUsernames();
                var users = new List<object>();

                // Build user list with quota information
                foreach (var username in allUsernames)
                {
                    var role = ActiveDirectory.GetUserRole(username);
                    var userQuota = await _userRepository.GetUserQuota(username);

                    var user = new
                    {
                        username = username,
                        fullName = GetFullName(username), // Map usernames to display names
                        role = role,
                        hasQuota = userQuota != null,
                        balance = userQuota?.GetBalanceInCHF() ?? 0.0
                    };
                    users.Add(user);
                }

                return Ok(new { success = true, users = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Map usernames to display names for UI
        private static string GetFullName(string username)
        {
            return username switch
            {
                "SCoppey" => "Stephane Coppey",
                "JDupont" => "Jean Dupont",
                "AMartin" => "Antoine Martin",
                "admin" => "System Administrator",
                "prof" => "Professor Account",
                "faculty" => "Faculty Administrator",
                "administrator" => "HES-SO Administrator",
                _ => $"User {username}"
            };
        }
    }
}