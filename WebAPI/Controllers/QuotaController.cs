using Microsoft.AspNetCore.Mvc;
using WebAPI.Interfaces;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotaController : ControllerBase
    {
        private readonly IQuotaService _quotaService;

        public QuotaController(IQuotaService quotaService)
        {
            _quotaService = quotaService;
        }

        /// <summary>
        /// Add CHF amount to user's print quota - used by both interfaces
        /// </summary>
        [HttpPost("add-amount")]
        public async Task<ActionResult<bool>> AddAmount([FromBody] AddAmountRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || request.ChfAmount <= 0)
                {
                    return BadRequest("Invalid username or amount");
                }

                bool result = await _quotaService.AddAmount(request.ChfAmount, request.Username);

                if (result)
                {
                    return Ok(new { success = true, message = $"Successfully added {request.ChfAmount} CHF to {request.Username}" });
                }

                return BadRequest(new { success = false, message = "Failed to add amount" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get user's balance in CHF - used by PoS interface
        /// </summary>
        [HttpGet("balance/{username}")]
        public async Task<ActionResult<double>> GetAvailableAmount(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Username is required");
                }

                double balance = await _quotaService.GetAvailableAmount(username);
                return Ok(new { username, balanceChf = balance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get all user balances - used by admin interface
        /// </summary>
        [HttpGet("all-balances")]
        public async Task<ActionResult> GetAllBalances()
        {
            try
            {
                var balances = await _quotaService.GetAllBalancesAsync();
                return Ok(balances);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Add semester quota - Faculty interface only
        /// </summary>
        [HttpPost("semester-quota")]
        public async Task<ActionResult<bool>> AddSemesterQuota([FromBody] AddSemesterQuotaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || request.ChfAmount <= 0)
                {
                    return BadRequest("Invalid username or amount");
                }

                bool result = await _quotaService.AddSemesterQuota(request.Username, request.ChfAmount);

                if (result)
                {
                    return Ok(new { success = true, message = $"Successfully added semester quota of {request.ChfAmount} CHF to {request.Username}" });
                }

                return BadRequest(new { success = false, message = "Failed to add semester quota" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Convert UID to username - for card scanner integration
        /// </summary>
        [HttpGet("username/{uid}")]
        public async Task<ActionResult<string>> GetUsername(string uid)
        {
            try
            {
                if (string.IsNullOrEmpty(uid))
                {
                    return BadRequest("UID is required");
                }

                string username = await _quotaService.GetUsername(uid);

                if (string.IsNullOrEmpty(username))
                {
                    return NotFound(new { error = "User not found for this UID" });
                }

                return Ok(new { uid, username });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    // API request models
    public class AddAmountRequest
    {
        public float ChfAmount { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public class AddSemesterQuotaRequest
    {
        public double ChfAmount { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    // API response model
    public class UserBalanceResponse
    {
        public string Username { get; set; } = string.Empty;
        public double BalanceChf { get; set; }
        public int Pages { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}