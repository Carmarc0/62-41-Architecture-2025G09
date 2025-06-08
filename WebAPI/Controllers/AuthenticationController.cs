using Microsoft.AspNetCore.Mvc;
using WebAPI.Interfaces;
using DAL.ExternalSystems;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Generic authentication endpoint for any user type
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult> Authenticate([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { success = false, message = "Username and password are required" });
                }

                bool isAuthenticated = await _authenticationService.Authenticate(request.Username, request.Password);

                if (isAuthenticated)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Authentication successful",
                        username = request.Username
                    });
                }
                else
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Invalid username or password"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Admin-specific authentication endpoint
        /// </summary>
        [HttpPost("login-admin")]
        public async Task<ActionResult> AuthenticateAdmin([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { success = false, message = "Username and password are required" });
                }

                bool isAuthenticated = await _authenticationService.AuthenticateAdmin(request.Username, request.Password);

                if (isAuthenticated)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Admin authentication successful",
                        username = request.Username
                    });
                }
                else
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Invalid admin credentials"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Student-specific authentication endpoint
        /// </summary>
        [HttpPost("login-student")]
        public async Task<ActionResult> AuthenticateStudent([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { success = false, message = "Username and password are required" });
                }

                bool isAuthenticated = await _authenticationService.AuthenticateStudent(request.Username, request.Password);

                if (isAuthenticated)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Student authentication successful",
                        username = request.Username
                    });
                }
                else
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Invalid student credentials"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Check if user exists in Active Directory
        /// </summary>
        [HttpGet("user-exists/{username}")]
        public async Task<ActionResult> CheckUserExists(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest(new { exists = false, message = "Username is required" });
                }

                bool exists = ActiveDirectory.UserExists(username);
                return Ok(new { exists = exists, username = username });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exists = false, message = ex.Message });
            }
        }
    }

    // Login request model
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}